using Breizh360.Domaine.Auth.Permissions;
using Breizh360.Domaine.Common;

namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-003 — RBAC + (optionnel) ABAC.
///
/// Règle de collaboration : pas de dynamic / reflection.
/// Dépendance attendue : <see cref="IPermissionRepository"/>.
/// </summary>
public sealed class AuthorizationServiceIsAllowed
{
    private readonly IPermissionRepository _permissionRepository;

    /// <summary>
    /// Constructeur typé (recommandé).
    /// </summary>
    public AuthorizationServiceIsAllowed(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
    }

    /// <summary>
    /// Constructeur legacy pour compatibilité.
    /// Si l'objet ne peut pas être casté en <see cref="IPermissionRepository"/>, une exception est levée.
    /// </summary>
    [Obsolete("Utiliser AuthorizationServiceIsAllowed(IPermissionRepository). Le constructeur object est conservé pour compatibilité.")]
    public AuthorizationServiceIsAllowed(object permissionRepository)
        : this(permissionRepository as IPermissionRepository
               ?? throw new ArgumentException(
                   "Le repository doit implémenter IPermissionRepository (Breizh360.Domaine.Auth.Permissions).",
                   nameof(permissionRepository)))
    {
    }

    /// <summary>
    /// Vérifie si <paramref name="userId"/> est autorisé pour une permission (code stable).
    ///
    /// Stratégie :
    /// - ABAC minimal : ":own" => ownership
    /// - RBAC : compare la permission demandée avec la liste effective côté Domaine (permissions via rôles)
    /// - Support simple des wildcards suffixes (ex: "admin.*")
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

        // Normalisation identique à celle du Domaine (trim + lower).
        var required = Normalization.NormalizeIdentityKey(permission);

        var perms = await _permissionRepository.ListForUserAsync(userId, ct).ConfigureAwait(false);
        if (perms is null || perms.Count == 0)
            return false;

        // Comparaison : exact puis wildcard.
        for (int i = 0; i < perms.Count; i++)
        {
            var code = perms[i].Code; // déjà normalisé côté Domaine

            if (string.Equals(code, required, StringComparison.OrdinalIgnoreCase))
                return true;

            if (MatchesWildcard(code, required))
                return true;
        }

        return false;
    }

    private static bool PassesAbac(Guid userId, string permission, AuthorizationContext? ctx)
    {
        // Exemple ownership : "x.y:own" => userId doit correspondre au owner
        if (ctx?.ResourceOwnerUserId is not null &&
            permission.EndsWith(":own", StringComparison.OrdinalIgnoreCase))
        {
            return ctx.ResourceOwnerUserId.Value == userId;
        }

        // Exemple tenant : si le Domaine gère des permissions par tenant,
        // ajouter ici une règle basée sur ctx.TenantId.
        return true;
    }

    /// <summary>
    /// Support minimal des wildcards suffixes :
    /// - "*" autorise tout
    /// - "admin.*" autorise "admin.read", "admin.write", etc.
    /// </summary>
    private static bool MatchesWildcard(string granted, string required)
    {
        if (string.IsNullOrWhiteSpace(granted)) return false;

        granted = granted.Trim();

        if (granted == "*")
            return true;

        if (!granted.EndsWith('*'))
            return false;

        var prefix = granted.TrimEnd('*');
        if (prefix.Length == 0) return true;

        return required.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }
}
