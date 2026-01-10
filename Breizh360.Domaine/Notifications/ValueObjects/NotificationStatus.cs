namespace Breizh360.Domaine.Notifications.ValueObjects;

/// <summary>
/// Statut de traitement / livraison de la notification.
/// La lecture (read/unread) est gérée séparément via <c>IsRead</c>.
/// </summary>
public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2,
    Cancelled = 3,
    Expired = 4
}
