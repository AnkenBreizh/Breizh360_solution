namespace Breizh360.Metier.Auth;

public sealed record AuthServiceValidateCredentialsResult<TUser>(
    bool IsValid,
    TUser? User
);
