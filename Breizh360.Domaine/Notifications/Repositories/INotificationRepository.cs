using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Breizh360.Domaine.Notifications.Repositories;

public interface INotificationRepository
{
    Task<Entities.Notification?> GetByIdAsync(ValueObjects.NotificationId id, CancellationToken ct = default);

    /// <summary>
    /// Récupère un lot de notifications à traiter (Pending et échéance atteinte).
    /// Le paramètre utcNow est passé par la couche applicative (testabilité).
    /// </summary>
    Task<IReadOnlyList<Entities.Notification>> FindPendingDueAsync(
        DateTime utcNow,
        int limit,
        CancellationToken ct = default);

    /// <summary>
    /// Anti-doublon (Optionnel mais recommandé si plusieurs sources peuvent produire la même notif).
    /// À faire respecter par l'implémentation Data (index unique ou contrainte).
    /// </summary>
    Task<bool> ExistsByIdempotencyKeyAsync(
        Guid userId,
        string idempotencyKey,
        CancellationToken ct = default);

    Task AddAsync(Entities.Notification notification, CancellationToken ct = default);
    Task UpdateAsync(Entities.Notification notification, CancellationToken ct = default);
}
