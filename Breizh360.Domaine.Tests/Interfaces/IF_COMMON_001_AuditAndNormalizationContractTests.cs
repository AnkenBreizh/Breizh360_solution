using System.Reflection;
using Breizh360.Tests.Shared;

namespace Breizh360.Domaine.Tests.Interfaces;

public class IF_COMMON_001_AuditAndNormalizationContractTests
{
    private readonly Assembly _dom = AssemblyLoader.LoadOrSkip("Breizh360.Domaine");

    [Fact]
    public void Should_expose_common_base_types()
    {
        var audit = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Common.AuditEntity");
        var ex    = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Common.DomainException");
        var norm  = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Common.Normalization");

        ReflectionAssert.ShouldBeClass(audit);
        ReflectionAssert.ShouldBeClass(ex);
        ReflectionAssert.ShouldBeClass(norm);

        typeof(Exception).IsAssignableFrom(ex).Should().BeTrue("DomainException doit hériter de Exception");
    }

    [Fact]
    public void Normalization_should_provide_NormalizeIdentityKey()
    {
        var norm = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Common.Normalization");

        var m = norm.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .FirstOrDefault(x => x.Name == "NormalizeIdentityKey" && x.GetParameters().Length == 1);

        m.Should().NotBeNull("Normalization.NormalizeIdentityKey(string) est utilisé dans le suivi (exemple de contrat)");
    }

    [Fact]
    public void AuditEntity_should_provide_expected_lifecycle_methods()
    {
        var audit = AssemblyLoader.GetTypeOrSkip(_dom, "Breizh360.Domaine.Common.AuditEntity");

        // Le suivi cite explicitement MarkCreated/MarkUpdated/SoftDelete/Restore (IF-COMMON-001)
        audit.GetMethods().Any(m => m.Name == "MarkCreated").Should().BeTrue();
        audit.GetMethods().Any(m => m.Name == "MarkUpdated").Should().BeTrue();
        audit.GetMethods().Any(m => m.Name == "SoftDelete" || m.Name == "SoftDeleteAsync").Should().BeTrue();
        audit.GetMethods().Any(m => m.Name == "Restore" || m.Name == "RestoreAsync").Should().BeTrue();
    }
}
