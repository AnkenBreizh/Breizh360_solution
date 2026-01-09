using Breizh360.Data.Auth.Repositories;
using Breizh360.Domaine.Auth.Permissions;
using Breizh360.Domaine.Auth.RefreshTokens;
using Breizh360.Domaine.Auth.Roles;
using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Breizh360.Data;

/// <summary>
/// Point d’entrée d’intégration DI pour la couche Données.
/// <para>
/// Objectif : rendre <c>Breizh360.Data</c> « plug-and-play » depuis la composition root
/// (API / Worker / Gateway) : DbContext + repositories + garde-fous.
/// </para>
/// </summary>
public static class Breizh360DataServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre <see cref="Breizh360DbContext"/> + repositories Auth (implémentations EF Core) dans la DI.
    /// </summary>
    /// <param name="services">Conteneur DI.</param>
    /// <param name="configuration">Configuration (pour récupérer la connection string).</param>
    /// <param name="configure">Options facultatives (nom de connection string, flags DEV...).</param>
    public static IServiceCollection AddBreizh360Data(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<Breizh360DataOptions>? configure = null)
    {
        var options = new Breizh360DataOptions();
        configure?.Invoke(options);

        var cs = configuration.GetConnectionString(options.ConnectionStringName);
        if (string.IsNullOrWhiteSpace(cs))
        {
            throw new InvalidOperationException(
                $"Missing connection string 'ConnectionStrings:{options.ConnectionStringName}'. " +
                "Configure it in appsettings.json / environment variables (ex: ConnectionStrings__Postgres)."
            );
        }

        // DbContext
        if (options.UseDbContextPooling)
        {
            services.AddDbContextPool<Breizh360DbContext>(db => ConfigureDb(db, cs, options));
        }
        else
        {
            services.AddDbContext<Breizh360DbContext>(db => ConfigureDb(db, cs, options));
        }

        // Repositories Auth (⚠️ attention à l’ambiguïté IUserRepository dans la solution)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }

    private static void ConfigureDb(DbContextOptionsBuilder db, string connectionString, Breizh360DataOptions options)
    {
        db.UseNpgsql(connectionString, npgsql =>
        {
            // Sécurise le scénario où le DbContext est enregistré depuis un projet hôte.
            npgsql.MigrationsAssembly(typeof(Breizh360DbContext).Assembly.GetName().Name);
        });

        if (options.EnableDetailedErrors)
            db.EnableDetailedErrors();

        if (options.EnableSensitiveDataLogging)
            db.EnableSensitiveDataLogging();
    }
}
