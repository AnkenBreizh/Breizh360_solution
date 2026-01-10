using System.Threading;
using System.Threading.Tasks;

namespace Breizh360.Domaine.Notifications.Senders;

public interface INotificationSender
{
    /// <summary>
    /// Envoi effectif sur le(s) canal(aux) cible(s).
    /// La persistance et les transitions de statut sont gérées par la couche applicative + domaine.
    /// </summary>
    Task SendAsync(Notifications.Entities.Notification notification, CancellationToken ct = default);
}
