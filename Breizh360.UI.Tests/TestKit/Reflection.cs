using System.Reflection;
using Xunit.Sdk;

namespace Breizh360.Tests.Shared;

public static class AssemblyLoader
{
    public static Assembly LoadOrSkip(string assemblyName)
    {
        try
        {
            return Assembly.Load(assemblyName);
        }
        catch (Exception ex)
        {
            throw new SkipException($"Assembly '{assemblyName}' introuvable (ou non buildé). Détail: {ex.GetType().Name}: {ex.Message}");
        }
    }

    public static Type GetTypeOrSkip(Assembly asm, string fullName)
    {
        var t = asm.GetType(fullName, throwOnError: false);
        if (t is null)
            throw new SkipException($"Type '{fullName}' introuvable dans l'assembly '{asm.GetName().Name}'.");
        return t;
    }
}

public static class ReflectionAssert
{
    public static MethodInfo FindMethod(Type type, string name, params Type[] parameterTypes)
    {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                          .Where(m => m.Name == name)
                          .ToArray();

        if (methods.Length == 0)
            throw new XunitException($"Méthode '{type.FullName}.{name}' introuvable.");

        foreach (var m in methods)
        {
            var ps = m.GetParameters().Select(p => p.ParameterType).ToArray();
            if (ps.SequenceEqual(parameterTypes))
                return m;
        }

        var sig = string.Join(", ", parameterTypes.Select(t => t.Name));
        var candidates = string.Join("\n", methods.Select(m => $"{m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))}) -> {m.ReturnType.Name}"));
        throw new XunitException($"Signature attendue non trouvée pour '{type.FullName}.{name}({sig})'. Candidats:\n{candidates}");
    }

    public static void ShouldBeInterface(Type t) => t.IsInterface.Should().BeTrue($"{t.FullName} doit être une interface");
    public static void ShouldBeClass(Type t) => t.IsClass.Should().BeTrue($"{t.FullName} doit être une classe");
}
