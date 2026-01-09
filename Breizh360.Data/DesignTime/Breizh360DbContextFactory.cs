using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Breizh360.Data.DesignTime;

public sealed class Breizh360DbContextFactory : IDesignTimeDbContextFactory<Breizh360DbContext>
{
    public Breizh360DbContext CreateDbContext(string[] args)
    {
        // 1) Essaye d’abord via env var (IIS / AppPool plus tard)
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres");

        // 2) Sinon, tente appsettings.Development.json du projet API (si présent)
        if (string.IsNullOrWhiteSpace(cs))
        {
            var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

            // Le projet hôte peut évoluer (ex: Breizh360.Api). On essaie plusieurs dossiers connus.
            var candidateApiPaths = new[]
            {
                Path.Combine(basePath, "Breizh360.Api"),
                Path.Combine(basePath, "Breizh360.Api.Metier"),
            };

            var apiPath = candidateApiPaths.FirstOrDefault(Directory.Exists) ?? basePath;

            var config = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            cs = config.GetConnectionString("Postgres");
        }

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Connection string 'ConnectionStrings:Postgres' introuvable.");

        var options = new DbContextOptionsBuilder<Breizh360DbContext>()
            .UseNpgsql(cs, b => b.MigrationsAssembly("Breizh360.Data"))
            .Options;

        return new Breizh360DbContext(options);
    }
}
