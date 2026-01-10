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
    private const string BearerPrefix = "Bearer ";

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

        if (IsAnonymousPath(path) || !IsGatewayProtectedPath(path))
        {
            await _next(context);
            return;
        }

        // 1) Authorization header (HTTP standard)
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var value = authHeader.ToString();
            if (!value.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                await RejectAsync(context, "AUTH_INVALID",
                    "Invalid Authorization header format (expected: Bearer <token>).",
                    log: $"Invalid Authorization header on {path}");
                return;
            }

            var token = value[BearerPrefix.Length..].Trim();
            if (!LooksLikeJwt(token))
            {
                await RejectAsync(context, "AUTH_INVALID",
                    "Token does not look like a JWT (expected 3 segments).",
                    log: $"Authorization token does not look like JWT on {path}");
                return;
            }

            await _next(context);
            return;
        }

        // 2) SignalR / WebSockets (navigateurs) : token peut transiter en querystring.
        //    IMPORTANT : l'API doit gérer l'extraction (ex: JwtBearer OnMessageReceived) pour les hubs.
        if (IsHubPath(path) && AllowAccessTokenQueryForHubs() &&
            context.Request.Query.TryGetValue("access_token", out var accessToken) &&
            !string.IsNullOrWhiteSpace(accessToken))
        {
            var token = accessToken.ToString();
            if (!LooksLikeJwt(token))
            {
                await RejectAsync(context, "AUTH_INVALID",
                    "access_token does not look like a JWT (expected 3 segments).",
                    log: $"access_token does not look like JWT on {path}");
                return;
            }

            await _next(context);
            return;
        }

        await RejectAsync(context, "AUTH_MISSING",
            "Missing token. Provide Authorization: Bearer <jwt> (or access_token query for hubs if enabled).",
            log: $"Missing token on {path}");
    }

    private bool IsAnonymousPath(string path)
    {
        var prefixes = _config.GetSection("Gateway:Auth:AnonymousPathPrefixes").Get<string[]>() ?? Array.Empty<string>();
        return prefixes.Any(p => !string.IsNullOrWhiteSpace(p) && path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsGatewayProtectedPath(string path)
        => path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) || IsHubPath(path);

    private static bool IsHubPath(string path)
        => path.StartsWith("/hubs/", StringComparison.OrdinalIgnoreCase);

    private bool AllowAccessTokenQueryForHubs()
        => _config.GetValue<bool?>("Gateway:Auth:AllowAccessTokenQueryForHubs") ?? true;

    private static bool LooksLikeJwt(string token)
    {
        // A very light heuristic: header.payload.signature
        var parts = token.Split('.');
        return parts.Length == 3 && parts.All(p => p.Length > 0);
    }

    private async Task RejectAsync(HttpContext context, string code, string message, string log)
    {
        _logger.LogWarning("{Log} (correlationId: {CorrelationId})", log, GetCorrelationId(context));

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json; charset=utf-8";

        var err = new GatewayError(code, message, GetCorrelationId(context));
        await context.Response.WriteAsJsonAsync(err);
    }

    private static string GetCorrelationId(HttpContext context)
    {
        var correlationId = context.Response.Headers[GatewayCorrelationIdMiddleware.HeaderName].ToString();
        if (!string.IsNullOrWhiteSpace(correlationId))
            return correlationId;

        correlationId = context.Request.Headers[GatewayCorrelationIdMiddleware.HeaderName].ToString();
        return !string.IsNullOrWhiteSpace(correlationId) ? correlationId : context.TraceIdentifier;
    }
}
