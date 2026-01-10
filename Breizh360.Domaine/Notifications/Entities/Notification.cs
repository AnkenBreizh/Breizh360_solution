using Breizh360.Domaine.Common;
using Breizh360.Domaine.Notifications.ValueObjects;

namespace Breizh360.Domaine.Notifications.Entities;

/// <summary>
/// Notification persistée (Inbox).
///
/// - Statut = traitement/livraison (Pending/Sent/Failed/Cancelled/Expired)
/// - Lecture = IsRead/ReadAtUtc
/// </summary>
public sealed class Notification
{
    public const int PayloadMaxLength = 32_000; // limite prudente, à ajuster côté Data si besoin

    public NotificationId Id { get; }

    /// <summary>
    /// Destinataire (identifiant technique). Nous utilisons Guid pour éviter un couplage fort au sous-domaine USR.
    /// </summary>
    public Guid UserId { get; }

    public NotificationType Type { get; }

    /// <summary>
    /// Contenu (souvent JSON) ou référence sérialisée.
    /// </summary>
    public string Payload { get; private set; }

    public NotificationStatus Status { get; private set; } = NotificationStatus.Pending;

    public bool IsRead { get; private set; }
    public DateTimeOffset? ReadAtUtc { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public DateTimeOffset? SentAtUtc { get; private set; }

    public int RetryCount { get; private set; }
    public DateTimeOffset? NextAttemptAtUtc { get; private set; }
    public DateTimeOffset? ExpiresAtUtc { get; private set; }

    public IdempotencyKey? IdempotencyKey { get; }

    private Notification(
        NotificationId id,
        Guid userId,
        NotificationType type,
        string payload,
        NotificationStatus status,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? nextAttemptAtUtc,
        DateTimeOffset? expiresAtUtc,
        IdempotencyKey? idempotencyKey)
    {
        if (id.Value == Guid.Empty) throw new DomainException("NotificationId ne peut pas être vide.");
        if (userId == Guid.Empty) throw new DomainException("UserId ne peut pas être vide.");

        Id = id;
        UserId = userId;
        Type = type;

        Payload = NormalizePayload(payload);
        Status = status;

        CreatedAtUtc = EnsureUtc(createdAtUtc);
        NextAttemptAtUtc = nextAttemptAtUtc is null ? null : EnsureUtc(nextAttemptAtUtc.Value);
        ExpiresAtUtc = expiresAtUtc is null ? null : EnsureUtc(expiresAtUtc.Value);

        if (ExpiresAtUtc is not null && ExpiresAtUtc <= CreatedAtUtc)
            throw new DomainException("ExpiresAtUtc doit être strictement supérieur à CreatedAtUtc.");

        IdempotencyKey = idempotencyKey;
    }

    /// <summary>
    /// Crée une notification persistée, initialement Pending.
    /// NextAttemptAtUtc par défaut = CreatedAtUtc (immédiat).
    /// </summary>
    public static Notification Create(
        NotificationId id,
        Guid userId,
        NotificationType type,
        string payload,
        IdempotencyKey? idempotencyKey = null,
        DateTimeOffset? createdAtUtc = null,
        DateTimeOffset? nextAttemptAtUtc = null,
        DateTimeOffset? expiresAtUtc = null)
    {
        var now = EnsureUtc(createdAtUtc ?? DateTimeOffset.UtcNow);

        return new Notification(
            id: id,
            userId: userId,
            type: type,
            payload: payload,
            status: NotificationStatus.Pending,
            createdAtUtc: now,
            nextAttemptAtUtc: EnsureUtc(nextAttemptAtUtc ?? now),
            expiresAtUtc: expiresAtUtc,
            idempotencyKey: idempotencyKey
        );
    }

    public void MarkSent(DateTimeOffset? sentAtUtc = null)
    {
        EnsureNotTerminal("MarkSent");

        Status = NotificationStatus.Sent;
        SentAtUtc = EnsureUtc(sentAtUtc ?? DateTimeOffset.UtcNow);
        NextAttemptAtUtc = null;

        Touch(SentAtUtc.Value);
    }

    /// <summary>
    /// Planifie une nouvelle tentative (garde le statut Pending).
    /// </summary>
    public void ScheduleRetry(DateTimeOffset nextAttemptAtUtc)
    {
        EnsureNotTerminal("ScheduleRetry");

        var next = EnsureUtc(nextAttemptAtUtc);

        if (next < CreatedAtUtc)
            throw new DomainException("NextAttemptAtUtc ne peut pas être antérieur à CreatedAtUtc.");

        RetryCount++;
        Status = NotificationStatus.Pending;
        NextAttemptAtUtc = next;

        Touch(next);
    }

    /// <summary>
    /// Marque la notification en échec définitif.
    /// </summary>
    public void MarkFailedTerminal(DateTimeOffset? failedAtUtc = null)
    {
        EnsureNotTerminal("MarkFailedTerminal");

        Status = NotificationStatus.Failed;
        NextAttemptAtUtc = null;

        Touch(EnsureUtc(failedAtUtc ?? DateTimeOffset.UtcNow));
    }

    public void Cancel(DateTimeOffset? nowUtc = null)
    {
        EnsureNotTerminal("Cancel");

        Status = NotificationStatus.Cancelled;
        NextAttemptAtUtc = null;

        Touch(EnsureUtc(nowUtc ?? DateTimeOffset.UtcNow));
    }

    public void Expire(DateTimeOffset? nowUtc = null)
    {
        EnsureNotTerminal("Expire");

        Status = NotificationStatus.Expired;
        NextAttemptAtUtc = null;

        Touch(EnsureUtc(nowUtc ?? DateTimeOffset.UtcNow));
    }

    public void MarkRead(DateTimeOffset? readAtUtc = null)
    {
        if (IsRead) return;

        IsRead = true;
        ReadAtUtc = EnsureUtc(readAtUtc ?? DateTimeOffset.UtcNow);

        Touch(ReadAtUtc.Value);
    }

    public void UpdatePayload(string payload, DateTimeOffset? nowUtc = null)
    {
        EnsureNotTerminal("UpdatePayload");

        Payload = NormalizePayload(payload);
        Touch(EnsureUtc(nowUtc ?? DateTimeOffset.UtcNow));
    }

    public bool IsExpired(DateTime utcNow)
    {
        if (ExpiresAtUtc is null) return false;

        var now = EnsureUtc(ToUtcOffset(utcNow));
        return ExpiresAtUtc <= now;
    }

    private void EnsureNotTerminal(string action)
    {
        if (Status is NotificationStatus.Sent or NotificationStatus.Failed or NotificationStatus.Cancelled or NotificationStatus.Expired)
            throw new DomainException($"Action '{action}' invalide : notification terminale ({Status}).");
    }

    private static string NormalizePayload(string payload)
    {
        payload = (payload ?? string.Empty).Trim();

        if (payload.Length == 0)
            throw new DomainException("Payload est requis.");

        if (payload.Length > PayloadMaxLength)
            throw new DomainException($"Payload est trop long (max {PayloadMaxLength}).");

        return payload;
    }

    private void Touch(DateTimeOffset nowUtc)
        => UpdatedAtUtc = EnsureUtc(nowUtc);

    private static DateTimeOffset EnsureUtc(DateTimeOffset value)
        => value.ToUniversalTime();

    private static DateTimeOffset ToUtcOffset(DateTime utc)
    {
        if (utc.Kind == DateTimeKind.Utc) return new DateTimeOffset(utc);
        if (utc.Kind == DateTimeKind.Local) return new DateTimeOffset(utc.ToUniversalTime());

        // Unspecified : on considère que la couche applicative a passé de l'UTC.
        return new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
    }
}
