namespace Breizh360.Domaine.Notifications.ValueObjects;

/// <summary>
/// Type fonctionnel de notification (ex: "usr.created", "auth.login").
/// </summary>
public readonly record struct NotificationType(string Value)
{
    public const int MaxLength = 64;

    public static NotificationType From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("NotificationType est requis.", nameof(value));

        value = value.Trim();

        if (value.Length > MaxLength)
            throw new ArgumentException($"NotificationType est trop long (max {MaxLength}).", nameof(value));

        return new NotificationType(value);
    }

    public override string ToString() => Value;
}
