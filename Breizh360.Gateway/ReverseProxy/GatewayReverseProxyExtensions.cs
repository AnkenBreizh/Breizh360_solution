using Breizh360.Gateway.Observability;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Transforms;

namespace Breizh360.Gateway.ReverseProxy;

/// <summary>
/// Configuration YARP centralisée pour la Passerelle.
/// </summary>
public static class GatewayReverseProxyExtensions
{
    /// <summary>
    /// Ajoute YARP et charge la config depuis la section "ReverseProxy".
    /// </summary>
    public static IReverseProxyBuilder AddGatewayReverseProxy(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"))
            .AddTransforms(transforms =>
            {
                // Propager systématiquement le X-Correlation-ID vers l'API.
                transforms.AddRequestTransform(ctx =>
                {
                    if (ctx.HttpContext.Request.Headers.TryGetValue(GatewayCorrelationIdMiddleware.HeaderName, out var cid))
                    {
                        ctx.ProxyRequest.Headers.Remove(GatewayCorrelationIdMiddleware.HeaderName);
                        ctx.ProxyRequest.Headers.TryAddWithoutValidation(GatewayCorrelationIdMiddleware.HeaderName, (string)cid);
                    }

                    return ValueTask.CompletedTask;
                });
            });
    }
}
