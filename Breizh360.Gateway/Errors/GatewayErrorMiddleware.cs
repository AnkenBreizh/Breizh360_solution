using Breizh360.Gateway.Observability;

namespace Breizh360.Gateway.Errors;

/// <summary>
/// Normalise uniquement les erreurs générées par la Passerelle elle-même
/// (exceptions dans la pipeline). Les erreurs downstream (API) sont forwardées as-is.
/// </summary>
public sealed class GatewayErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayErrorMiddleware> _logger;

    public GatewayErrorMiddleware(RequestDelegate next, ILogger<GatewayErrorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in Gateway pipeline.");
            if (context.Response.HasStarted)
                throw;

            await WriteAsync(context, StatusCodes.Status502BadGateway, "GW_UNHANDLED", "Gateway error.");
        }
    }

    private static async Task WriteAsync(HttpContext context, int status, string code, string message)
    {
        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json; charset=utf-8";

        var correlationId = context.Response.Headers[GatewayCorrelationIdMiddleware.HeaderName].ToString();
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            // Au cas où le header n'a pas encore été fixé (ex: exception très tôt).
            correlationId = context.Request.Headers[GatewayCorrelationIdMiddleware.HeaderName].ToString();
        }

        var err = new GatewayError(code, message, correlationId);
        await context.Response.WriteAsJsonAsync(err);
    }
}
