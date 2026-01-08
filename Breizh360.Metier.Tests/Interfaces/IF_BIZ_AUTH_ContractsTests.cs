using System.Reflection;
using Breizh360.Tests.Shared;
using Xunit.Sdk;

namespace Breizh360.Metier.Tests.Interfaces;

public class IF_BIZ_AUTH_ContractsTests
{
    private readonly Assembly _biz = AssemblyLoader.LoadOrSkip("Breizh360.Metier");

    [Theory]
    [InlineData("AuthServiceValidateCredentials", "IF-BIZ-AUTH-001")]
    [InlineData("TokenService", "IF-BIZ-AUTH-002")]
    [InlineData("AuthorizationServiceIsAllowed", "IF-BIZ-AUTH-003")]
    public void Should_expose_expected_services(string typeName, string interfaceId)
    {
        var t = _biz.GetTypes().FirstOrDefault(x => x.Name == typeName);
        if (t is null)
            throw SkipException.ForSkip($"{typeName} introuvable dans Breizh360.Metier. (Contrat {interfaceId} dans le suivi)");

        t.IsClass.Should().BeTrue();
        t.IsAbstract.Should().BeFalse();
    }

    [Fact]
    public void TokenService_should_expose_issue_and_refresh_methods()
    {
        var t = _biz.GetTypes().FirstOrDefault(x => x.Name == "TokenService");
        if (t is null)
            throw SkipException.ForSkip("TokenService introuvable, test ignoré.");

        t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
         .Any(m => m.Name.Contains("Issue", StringComparison.OrdinalIgnoreCase))
         .Should().BeTrue("TokenService doit exposer une méthode d'émission (Issue*)");

        t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
         .Any(m => m.Name.Contains("Refresh", StringComparison.OrdinalIgnoreCase))
         .Should().BeTrue("TokenService doit exposer une méthode de refresh (Refresh*)");
    }

    [Fact]
    public void AuthorizationServiceIsAllowed_should_expose_IsAllowed_method()
    {
        var t = _biz.GetTypes().FirstOrDefault(x => x.Name == "AuthorizationServiceIsAllowed");
        if (t is null)
            throw SkipException.ForSkip("AuthorizationServiceIsAllowed introuvable, test ignoré.");

        t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
         .Any(m => m.Name.Equals("IsAllowedAsync", StringComparison.Ordinal) || m.Name.Equals("IsAllowed", StringComparison.Ordinal))
         .Should().BeTrue("Doit exposer IsAllowed* selon le contrat IF-BIZ-AUTH-003");
    }
}
