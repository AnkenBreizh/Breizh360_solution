using Microsoft.Extensions.Logging;

namespace Breizh360.Gateway.Common;

/// <summary>
/// Centralise la configuration des logs de la Passerelle.
/// </summary>
public static class CommonLoggingExtensions
{
    /// <summary>
    /// Configure les providers de log (Console).
    /// </summary>
    public static WebApplicationBuilder AddGatewayLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        return builder;
    }
}
