using Breizh360.Domaine.Common;
using Breizh360.Domaine.Users.ValueObjects;

namespace Breizh360.Domaine.Users.Entities;

/// <summary>
/// Aggregate root User (profil minimal).
/// </summary>
public sealed class User
{
    public UserId Id { get; }
    public Email Email { get; private set; }
    public DisplayName DisplayName { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; }

    private User(UserId id, Email email, DisplayName displayName, DateTimeOffset createdAtUtc)
    {
        Id = id;
        Email = email;
        DisplayName = displayName;
        CreatedAtUtc = createdAtUtc;
    }

    public static User Create(UserId id, Email email, DisplayName displayName, DateTimeOffset? createdAtUtc = null)
    {
        if (id.Value == Guid.Empty)
            throw new DomainException("UserId ne peut pas être vide.");

        return new User(
            id: id,
            email: email,
            displayName: displayName,
            createdAtUtc: createdAtUtc ?? DateTimeOffset.UtcNow
        );
    }

    public void ChangeDisplayName(DisplayName displayName)
        => DisplayName = displayName;

    public void ChangeEmail(Email email)
        => Email = email;
}
