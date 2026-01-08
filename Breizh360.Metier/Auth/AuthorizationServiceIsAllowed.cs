namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-003 — RBAC + (optionnel) ABAC.
/// </summary>
public sealed class AuthorizationServiceIsAllowed
{
    private readonly object _authorizationRepository;

    public AuthorizationServiceIsAllowed(object authorizationRepository)
    {
        _authorizationRepository = authorizationRepository ?? throw new ArgumentNullException(nameof(authorizationRepository));
    }

    /// <summary>
    /// Vérifie si <paramref name="userId"/> est autorisé pour la permission.
    /// Stratégie :
    /// 1) si le dépôt expose IsAllowedAsync/HasPermissionAsync, on délègue
    /// 2) sinon, on récupère la liste des permissions et on fait la vérif ici
    /// 3) ABAC minimal : *:own => ownership
    /// </summary>
    public async Task<bool> IsAllowedAsync(
        Guid userId,
        string permission,
        AuthorizationContext? ctx = null,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty) return false;
        if (string.IsNullOrWhiteSpace(permission)) return false;

        // ABAC minimal (ownership) : si non respecté => refus immédiat
        if (!PassesAbac(userId, permission, ctx))
            return false;

        // 1) délégation directe
        var direct = await RepoInvoke.CallFirstAsync<bool?>(
            _authorizationRepository,
            new[]
            {
                "IsAllowedAsync",
                "HasPermissionAsync",
                "CheckAsync"
            },
            new object?[] { userId, permission, ctx },
            ct
        ).ConfigureAwait(false);

        if (direct is not null)
            return direct.Value;

        // 2) liste des permissions
        var perms = await RepoInvoke.CallFirstAsync<IEnumerable<string>>(
            _authorizationRepository,
            new[]
            {
                "GetPermissionsForUserAsync",
                "ListPermissionsAsync",
                "GetEffectivePermissionsAsync"
            },
            new object?[] { userId },
            ct
        ).ConfigureAwait(false);

        if (perms is null)
            return false;

        return perms.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    private static bool PassesAbac(Guid userId, string permission, AuthorizationContext? ctx)
    {
        // Exemple ownership : "x.y:own" => userId doit correspondre au owner
        if (ctx?.ResourceOwnerUserId is not null &&
            permission.EndsWith(":own", StringComparison.OrdinalIgnoreCase))
        {
            return ctx.ResourceOwnerUserId.Value == userId;
        }

        // Exemple tenant : si ton Domaine gère des permissions par tenant, tu peux ajouter ici une règle
        // basée sur ctx.TenantId. On ne force rien tant que le contrat n'est pas stabilisé.
        return true;
    }
}
