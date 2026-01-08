namespace Breizh360.Gateway.Observability;

public sealed class GatewayCorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayCorrelationIdMiddleware> _logger;

    public GatewayCorrelationIdMiddleware(RequestDelegate next, ILogger<GatewayCorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // Response header (always)
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var cid) && !string.IsNullOrWhiteSpace(cid))
        {
            return cid.ToString();
        }

        // Prefer TraceIdentifier when available for consistency, else generate a GUID.
        var generated = !string.IsNullOrWhiteSpace(context.TraceIdentifier)
            ? context.TraceIdentifier
            : Guid.NewGuid().ToString("N");

        context.Request.Headers[HeaderName] = generated;
        return generated;
    }
}
