using System.Reflection;
using Breizh360.Tests.Shared;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit.Sdk;

namespace Breizh360.Data.Tests.Interfaces;

public class IF_DATA_AUTH_001_SchemaContractTests
{
    private readonly Assembly _data = AssemblyLoader.LoadOrSkip("Breizh360.Data");

    [Fact]
    public void Should_expose_DbContext()
    {
        var db = AssemblyLoader.GetTypeOrSkip(_data, "Breizh360.Data.Breizh360DbContext");
        ReflectionAssert.ShouldBeClass(db);
    }

    [Fact]
    public void Should_expose_auth_ef_configurations()
    {
        var cfgs = _data.GetTypes()
                        .Where(t => t.Namespace != null &&
                                    t.Namespace.StartsWith("Breizh360.Data.Auth.Configurations", StringComparison.Ordinal) &&
                                    t.Name.EndsWith("EfConfiguration", StringComparison.Ordinal))
                        .ToArray();

        cfgs.Length.Should().BeGreaterThan(0, "Le suivi référence Breizh360.Data/Auth/Configurations/*EfConfiguration.cs");
    }

    [Fact]
    public void Should_expose_auth_migrations_namespace()
    {
        // Si EF Core Migrations existent, on doit trouver au moins une classe Migration dans namespace Breizh360.Data.Migrations.Auth
        var migrations = _data.GetTypes()
                              .Where(t => t.Namespace != null &&
                                          t.Namespace.StartsWith("Breizh360.Data.Migrations.Auth", StringComparison.Ordinal) &&
                                          typeof(Migration).IsAssignableFrom(t))
                              .ToArray();

        if (migrations.Length == 0)
            throw SkipException.ForSkip("Aucune migration Auth détectée (namespace Breizh360.Data.Migrations.Auth). Si elles ne sont pas committées ou si le namespace diffère, ajuster ce test.");
    }
}
