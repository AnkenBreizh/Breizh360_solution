namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-003 — RBAC + (optionnel) ABAC.
/// Remise attendue : Breizh360.Metier/Auth/AuthorizationServiceIsAllowed.cs
/// </summary>
public sealed class AuthorizationServiceIsAllowed
{
    // TODO: remplacer par les interfaces Domaine (ex: IUserRepository / IRoleRepository / IPermissionRepository)
    private readonly dynamic _authorizationRepository;

    public AuthorizationServiceIsAllowed(dynamic authorizationRepository)
    {
        _authorizationRepository = authorizationRepository;
    }

    public async Task<bool> IsAllowedAsync(
        Guid userId,
        string permission,
        AuthorizationContext? ctx,
        CancellationToken ct)
    {
        // --- RBAC ---
        // TODO: récupérer les permissions effectives de l’utilisateur via Domaine/Data
        bool hasPermission = await _authorizationRepository.UserHasPermissionAsync(userId, permission, ct);
        if (!hasPermission)
            return false;

        // --- ABAC (minimal, extensible) ---
        // Exemple : si une permission implique ownership, vérifier ctx.ResourceOwnerUserId
        // (à adapter aux règles ABAC réellement modélisées dans ton Domaine)
        if (ctx?.ResourceOwnerUserId is not null)
        {
            // TODO: exemple : permission "X:own" => userId doit être owner
            if (permission.EndsWith(":own", StringComparison.OrdinalIgnoreCase))
                return ctx.ResourceOwnerUserId.Value == userId;
        }

        return true;
    }
}
