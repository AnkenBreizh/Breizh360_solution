namespace Breizh360.Metier.Auth;

public abstract class AuthException : Exception
{
    protected AuthException(string message) : base(message) { }
}

public sealed class AuthExceptionInvalidCredentials : AuthException
{
    public AuthExceptionInvalidCredentials() : base("Identifiants invalides.") { }
}

public sealed class AuthExceptionUserLocked : AuthException
{
    public DateTimeOffset? LockedUntilUtc { get; }

    public AuthExceptionUserLocked(DateTimeOffset? lockedUntilUtc)
        : base("Compte verrouillé.")
    {
        LockedUntilUtc = lockedUntilUtc;
    }
}
