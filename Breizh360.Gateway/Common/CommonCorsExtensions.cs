using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Breizh360.Gateway.Common;

/// <summary>
/// Configuration CORS centralisée pour la Passerelle.
/// </summary>
public static class CommonCorsExtensions
{
    public const string PolicyName = "gateway-cors";

    public static IServiceCollection AddGatewayCors(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                var origins = configuration.GetSection("Gateway:Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

                if (origins.Length == 0)
                {
                    if (environment.IsDevelopment())
                    {
                        // Dev fallback : autoriser dynamiquement l'origine (pas de wildcard) + credentials.
                        // IMPORTANT : en prod, renseigner explicitement AllowedOrigins.
                        policy.SetIsOriginAllowed(_ => true);
                    }
                    else
                    {
                        // Sécurité : si la liste n'est pas renseignée en non-dev, on refuse toute origine.
                        policy.SetIsOriginAllowed(_ => false);
                    }
                }
                else
                {
                    policy.WithOrigins(origins);
                }

                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }
}
