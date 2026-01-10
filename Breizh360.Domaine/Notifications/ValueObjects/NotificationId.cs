namespace Breizh360.Domaine.Notifications.ValueObjects;

/// <summary>
/// Identifiant fort pour une notification.
/// </summary>
public readonly record struct NotificationId(Guid Value)
{
    public static NotificationId New() => new(Guid.NewGuid());

    public static NotificationId From(Guid value)
        => value == Guid.Empty
            ? throw new ArgumentException("NotificationId ne peut pas être vide.", nameof(value))
            : new NotificationId(value);

    public override string ToString() => Value.ToString();
}
