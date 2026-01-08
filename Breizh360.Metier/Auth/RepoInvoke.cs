using System.Reflection;
using System.Threading.Tasks;

namespace Breizh360.Metier.Auth;

internal static class RepoInvoke
{
    /// <summary>
    /// Tente d'invoquer une méthode async sur un "repository" (objet injecté depuis Domaine/Data),
    /// sans dépendre de ses types/namespace au compile-time.
    /// </summary>
    public static async Task<(bool Found, T? Value)> TryCallAsync<T>(
        object repository,
        string methodName,
        object?[] args,
        CancellationToken ct = default)
    {
        if (repository is null) throw new ArgumentNullException(nameof(repository));

        var repoType = repository.GetType();
        var candidates = repoType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => string.Equals(m.Name, methodName, StringComparison.Ordinal)
                     || string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        foreach (var m in candidates)
        {
            if (!TryBuildInvokeArgs(m, args, ct, out var invokeArgs))
                continue;

            try
            {
                var raw = m.Invoke(repository, invokeArgs);
                var awaited = await AwaitMaybeAsync(raw).ConfigureAwait(false);

                if (awaited is null)
                    return (true, default);

                if (awaited is T t)
                    return (true, t);

                // Support conversion for common cases (e.g. IEnumerable<string> -> List<string>)
                if (typeof(T) == typeof(IEnumerable<string>) && awaited is IEnumerable<string> s)
                    return (true, (T?)(object)s);

                return (true, (T?)awaited);
            }
            catch (TargetInvocationException tie) when (tie.InnerException is not null)
            {
                // Propager l'exception métier d'origine
                throw tie.InnerException;
            }
        }

        return (false, default);
    }

    public static async Task<T?> CallFirstAsync<T>(
        object repository,
        IEnumerable<string> methodNames,
        object?[] args,
        CancellationToken ct = default)
    {
        foreach (var name in methodNames)
        {
            var (found, value) = await TryCallAsync<T>(repository, name, args, ct).ConfigureAwait(false);
            if (found)
                return value;
        }
        return default;
    }

    public static bool TryGetProperty<T>(object obj, IEnumerable<string> names, out T? value)
    {
        value = default;
        if (obj is null) return false;

        var type = obj.GetType();

        foreach (var name in names)
        {
            var p = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (p is null) continue;

            var raw = p.GetValue(obj);
            if (raw is null)
            {
                value = default;
                return true;
            }

            if (raw is T t)
            {
                value = t;
                return true;
            }

            try
            {
                value = (T?)Convert.ChangeType(raw, typeof(T));
                return true;
            }
            catch
            {
                // ignore conversion issues, continue trying other names
            }
        }

        return false;
    }

    private static bool TryBuildInvokeArgs(MethodInfo m, object?[] args, CancellationToken ct, out object?[] invokeArgs)
    {
        invokeArgs = Array.Empty<object?>();

        var ps = m.GetParameters();
        if (ps.Length != args.Length && ps.Length != args.Length + 1)
            return false;

        // match base args
        var tmp = new object?[ps.Length];

        for (int i = 0; i < args.Length; i++)
        {
            var paramType = ps[i].ParameterType;
            var provided = args[i];

            if (provided is null)
            {
                tmp[i] = null;
                continue;
            }

            if (paramType.IsInstanceOfType(provided))
            {
                tmp[i] = provided;
                continue;
            }

            // allow Guid/string/etc conversions where safe-ish
            try
            {
                tmp[i] = Convert.ChangeType(provided, paramType);
            }
            catch
            {
                return false;
            }
        }

        if (ps.Length == args.Length + 1)
        {
            // last param must be CancellationToken
            if (ps[^1].ParameterType != typeof(CancellationToken))
                return false;
            tmp[^1] = ct;
        }

        invokeArgs = tmp;
        return true;
    }

    private static async Task<object?> AwaitMaybeAsync(object? maybeTask)
    {
        if (maybeTask is null) return null;

        if (maybeTask is Task t)
        {
            await t.ConfigureAwait(false);
            return GetTaskResult(t);
        }

        var type = maybeTask.GetType();
        if (type.FullName is not null && type.FullName.StartsWith("System.Threading.Tasks.ValueTask", StringComparison.Ordinal))
        {
            var asTask = type.GetMethod("AsTask", BindingFlags.Instance | BindingFlags.Public, binder: null, Type.EmptyTypes, modifiers: null);
            if (asTask is not null)
            {
                var taskObj = asTask.Invoke(maybeTask, null);
                return await AwaitMaybeAsync(taskObj).ConfigureAwait(false);
            }
        }

        return maybeTask;
    }

    private static object? GetTaskResult(Task task)
    {
        var type = task.GetType();
        if (!type.IsGenericType)
            return null;

        var prop = type.GetProperty("Result", BindingFlags.Instance | BindingFlags.Public);
        return prop?.GetValue(task);
    }
}
