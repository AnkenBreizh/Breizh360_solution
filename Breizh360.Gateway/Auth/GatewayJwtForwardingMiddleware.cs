using Breizh360.Gateway.Errors;
using Breizh360.Gateway.Observability;

namespace Breizh360.Gateway.Auth;

/// <summary>
/// Gateway is NOT the source of authority for JWT signature/claims validation.
/// This middleware only enforces presence/format rules on protected routes.
/// The downstream API performs real authentication/authorization.
/// </summary>
public sealed class GatewayJwtForwardingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;
    private readonly ILogger<GatewayJwtForwardingMiddleware> _logger;

    public GatewayJwtForwardingMiddleware(RequestDelegate next, IConfiguration config, ILogger<GatewayJwtForwardingMiddleware> logger)
    {
        _next = next;
        _config = config;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (IsAnonymousPath(path))
        {
            await _next(context);
            return;
        }

        // Only protect proxied API/Hub paths
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/hubs/", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            await RejectAsync(context, "AUTH_MISSING", "Missing Authorization header (Bearer token required).");
            return;
        }

        var value = authHeader.ToString();
        if (!value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await RejectAsync(context, "AUTH_INVALID", "Invalid Authorization header format (expected: Bearer <token>).");
            return;
        }

        var token = value["Bearer ".Length..].Trim();
        if (!LooksLikeJwt(token))
        {
            await RejectAsync(context, "AUTH_INVALID", "Token does not look like a JWT (expected 3 segments).");
            return;
        }

        await _next(context);
    }

    private bool IsAnonymousPath(string path)
    {
        var prefixes = _config.GetSection("Gateway:Auth:AnonymousPathPrefixes").Get<string[]>() ?? Array.Empty<string>();
        return prefixes.Any(p => !string.IsNullOrWhiteSpace(p) && path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private static bool LooksLikeJwt(string token)
    {
        // A very light heuristic: header.payload.signature
        var parts = token.Split('.');
        return parts.Length == 3 && parts.All(p => p.Length > 0);
    }

    private static async Task RejectAsync(HttpContext context, string code, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json; charset=utf-8";

        var correlationId = context.Response.Headers[GatewayCorrelationIdMiddleware.HeaderName].ToString();
        var err = new GatewayError(code, message, correlationId);

        await context.Response.WriteAsJsonAsync(err);
    }
}
