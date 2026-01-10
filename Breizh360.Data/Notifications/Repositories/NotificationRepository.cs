using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Breizh360.Domaine.Notifications.Repositories;
using Breizh360.Domaine.Notifications.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Notifications.Repositories;

/// <summary>
/// Implémentation EF Core du repository Inbox Notifications.
/// </summary>
public sealed class NotificationRepository : INotificationRepository
{
    private readonly Breizh360DbContext _db;

    public NotificationRepository(Breizh360DbContext db) => _db = db;

    public Task<Domaine.Notifications.Entities.Notification?> GetByIdAsync(
        NotificationId id,
        CancellationToken ct = default)
        => _db.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<IReadOnlyList<Domaine.Notifications.Entities.Notification>> FindPendingDueAsync(
        DateTime utcNow,
        int limit,
        CancellationToken ct = default)
    {
        if (limit <= 0) return Array.Empty<Domaine.Notifications.Entities.Notification>();

        // Le contrat Domaine passe un DateTime (testabilité). Nous le traitons comme UTC.
        var nowUtc = utcNow.Kind == DateTimeKind.Utc
            ? new DateTimeOffset(utcNow)
            : new DateTimeOffset(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc));

        return await _db.Notifications
            .Where(n => n.Status == Domaine.Notifications.ValueObjects.NotificationStatus.Pending)
            .Where(n => n.NextAttemptAtUtc != null && n.NextAttemptAtUtc <= nowUtc)
            .OrderBy(n => n.NextAttemptAtUtc)
            .ThenBy(n => n.CreatedAtUtc)
            .Take(limit)
            .ToListAsync(ct);
    }

    public Task<bool> ExistsByIdempotencyKeyAsync(
        Guid userId,
        string idempotencyKey,
        CancellationToken ct = default)
    {
        var key = IdempotencyKey.From(idempotencyKey);

        return _db.Notifications.AnyAsync(
            n => n.UserId == userId && n.IdempotencyKey == key,
            ct);
    }

    public Task AddAsync(Domaine.Notifications.Entities.Notification notification, CancellationToken ct = default)
        => _db.Notifications.AddAsync(notification, ct).AsTask();

    public Task UpdateAsync(Domaine.Notifications.Entities.Notification notification, CancellationToken ct = default)
    {
        _db.Notifications.Update(notification);
        return Task.CompletedTask;
    }
}
