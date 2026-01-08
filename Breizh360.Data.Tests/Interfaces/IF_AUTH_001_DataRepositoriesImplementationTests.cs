using System.Reflection;
using Breizh360.Tests.Shared;
using Xunit.Sdk;

namespace Breizh360.Data.Tests.Interfaces;

public class IF_AUTH_001_DataRepositoriesImplementationTests
{
    private readonly Assembly _data = AssemblyLoader.LoadOrSkip("Breizh360.Data");
    private readonly Assembly _dom  = AssemblyLoader.LoadOrSkip("Breizh360.Domaine");

    [Fact]
    public void PermissionRepository_should_implement_IPermissionRepository()
    {
        var permRepo = _data.GetType("Breizh360.Data.Auth.Repositories.PermissionRepository", throwOnError: false);
        if (permRepo is null)
            throw SkipException.ForSkip("PermissionRepository introuvable (Breizh360.Data.Auth.Repositories.PermissionRepository).");

        var permItf = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Auth.Permissions.IPermissionRepository");

        permItf.IsAssignableFrom(permRepo).Should().BeTrue("PermissionRepository doit implémenter IPermissionRepository");

        // Vérifie aussi les méthodes explicitement attendues (cf suivi / erreurs CS0535)
        permRepo.GetMethod("GetByIdAsync")     .Should().NotBeNull();
        permRepo.GetMethod("GetByCodeAsync")   .Should().NotBeNull();
        permRepo.GetMethod("ListAsync")        .Should().NotBeNull();
    }
}
