using System.Reflection;
using System.Threading;
using Breizh360.Tests.Shared;

namespace Breizh360.Domaine.Tests.Interfaces;

public class IF_AUTH_001_RepositoryContractsTests
{
    private readonly Assembly _dom = AssemblyLoader.LoadOrSkip("Breizh360.Domaine");

    [Fact]
    public void Should_expose_repository_interfaces()
    {
        var userRepo = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Auth.Users.IUserRepository");
        var roleRepo = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Auth.Roles.IRoleRepository");
        var permRepo = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Auth.Permissions.IPermissionRepository");
        var rtRepo   = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Auth.RefreshTokens.IRefreshTokenRepository");

        ReflectionAssert.ShouldBeInterface(userRepo);
        ReflectionAssert.ShouldBeInterface(roleRepo);
        ReflectionAssert.ShouldBeInterface(permRepo);
        ReflectionAssert.ShouldBeInterface(rtRepo);
    }

    [Fact]
    public void IPermissionRepository_should_contain_expected_methods_from_suivi()
    {
        // Rappel suivi: AUTH-REQ-004 / AUTH-DATA-002 mentionne explicitement ces signatures
        var permRepo = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Auth.Permissions.IPermissionRepository");

        ReflectionAssert.FindMethod(permRepo, "GetByIdAsync", typeof(Guid), typeof(CancellationToken));
        ReflectionAssert.FindMethod(permRepo, "GetByCodeAsync", typeof(string), typeof(CancellationToken));

        // ListAsync: certaines implémentations ajoutent un param optionnel; on accepte 0 ou 1 param CancellationToken
        var listMethods = permRepo.GetMethods().Where(m => m.Name == "ListAsync").ToArray();
        listMethods.Length.Should().BeGreaterThan(0, "ListAsync doit exister dans IPermissionRepository");

        var ok = listMethods.Any(m =>
        {
            var ps = m.GetParameters();
            return ps.Length == 0 || (ps.Length == 1 && ps[0].ParameterType == typeof(CancellationToken));
        });

        ok.Should().BeTrue("ListAsync doit avoir 0 paramètre ou un CancellationToken");
    }
}
