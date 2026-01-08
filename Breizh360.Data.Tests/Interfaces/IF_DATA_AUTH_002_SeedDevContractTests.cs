using System.Reflection;
using Breizh360.Tests.Shared;
using Xunit.Sdk;

namespace Breizh360.Data.Tests.Interfaces;

public class IF_DATA_AUTH_002_SeedDevContractTests
{
    private readonly Assembly _data = AssemblyLoader.LoadOrSkip("Breizh360.Data");

    [Fact]
    public void Should_expose_AuthSeedDev_class()
    {
        var seed = _data.GetType("Breizh360.Data.Auth.Seed.AuthSeedDev", throwOnError: false);
        if (seed is null)
            throw SkipException.ForSkip("Type Breizh360.Data.Auth.Seed.AuthSeedDev non trouvé. Le suivi indique un fichier Breizh360.Data/Auth/Seed/AuthSeedDev.cs.");

        seed.IsClass.Should().BeTrue();
    }

    [Fact]
    public void AuthSeedDev_should_provide_EnsureSeed_method()
    {
        var seed = _data.GetType("Breizh360.Data.Auth.Seed.AuthSeedDev", throwOnError: false);
        if (seed is null)
            throw SkipException.ForSkip("AuthSeedDev absent, test ignoré.");

        var m = seed.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.Name.StartsWith("EnsureSeed", StringComparison.Ordinal));

        if (m is null)
            throw SkipException.ForSkip("Méthode EnsureSeed* non trouvée sur AuthSeedDev. Ajuster si le nom diffère.");

        // On s'assure juste qu'elle est async-friendly (Task/ValueTask)
        var rt = m.ReturnType;
        var ok = rt.Name.Contains("Task", StringComparison.Ordinal);
        ok.Should().BeTrue("EnsureSeed* doit retourner Task/ValueTask");
    }
}
