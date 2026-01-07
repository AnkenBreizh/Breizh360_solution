namespace Breizh360.Metier.Auth;

public sealed record AuthorizationContext(
    Guid? TenantId = null,
    Guid? ResourceOwnerUserId = null
);
