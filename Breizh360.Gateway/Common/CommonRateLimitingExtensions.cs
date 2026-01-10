using System.Threading.RateLimiting;
using Breizh360.Gateway.Errors;
using Breizh360.Gateway.Observability;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Breizh360.Gateway.Common;

/// <summary>
/// Rate limiting (par IP) + réponse 429 normalisée au format GatewayError.
/// </summary>
public static class CommonRateLimitingExtensions
{
    public const string PolicyName = "gw-fixed";

    public static IServiceCollection AddGatewayRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var permitLimit = configuration.GetValue<int?>("Gateway:RateLimiting:PermitLimit") ?? 200;
        var windowSeconds = configuration.GetValue<int?>("Gateway:RateLimiting:WindowSeconds") ?? 60;

        services.AddRateLimiter(options =>
        {
            options.AddPolicy(PolicyName, httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromSeconds(windowSeconds),
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    AutoReplenishment = true
                });
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Émettre un body JSON cohérent, plutôt qu'un 429 vide.
            options.OnRejected = async (context, cancellationToken) =>
            {
                var http = context.HttpContext;
                if (http.Response.HasStarted)
                    return;

                http.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                http.Response.ContentType = "application/json; charset=utf-8";

                var correlationId = GetCorrelationId(http);
                var err = new GatewayError("RATE_LIMITED", "Too many requests.", correlationId);

                await http.Response.WriteAsJsonAsync(err, cancellationToken: cancellationToken);
            };
        });

        return services;
    }

    private static string GetCorrelationId(HttpContext http)
    {
        if (http.Response.Headers.TryGetValue(GatewayCorrelationIdMiddleware.HeaderName, out var cidResponse) && !string.IsNullOrWhiteSpace(cidResponse))
            return cidResponse.ToString();

        if (http.Request.Headers.TryGetValue(GatewayCorrelationIdMiddleware.HeaderName, out var cidRequest) && !string.IsNullOrWhiteSpace(cidRequest))
            return cidRequest.ToString();

        return http.TraceIdentifier;
    }
}
