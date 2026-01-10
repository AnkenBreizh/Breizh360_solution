using Breizh360.Gateway.Observability;
using Microsoft.AspNetCore.Builder;

namespace Breizh360.Gateway.Common;

/// <summary>
/// Helpers de pipeline pour la corrélation.
/// </summary>
public static class CommonCorrelationIdMiddleware
{
    public static IApplicationBuilder UseGatewayCorrelationId(this IApplicationBuilder app)
        => app.UseMiddleware<GatewayCorrelationIdMiddleware>();
}
