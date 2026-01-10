using Breizh360.Domaine.Notifications.Entities;
using Breizh360.Domaine.Notifications.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Notifications.Configurations;

/// <summary>
/// Mapping EF Core pour la notification persistée (Inbox).
///
/// Remarque : l'entité Domaine utilise des ValueObjects (NotificationId / NotificationType / IdempotencyKey).
/// Nous utilisons donc des conversions de valeur EF Core.
/// </summary>
public sealed class NotificationEfConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("notif_inbox_notifications");

        // --- Key / Identity
        b.HasKey(n => n.Id);

        b.Property(n => n.Id)
            .HasConversion(
                id => id.Value,
                value => NotificationId.From(value))
            .ValueGeneratedNever();

        // --- Columns
        b.Property(n => n.UserId).IsRequired();

        b.Property(n => n.Type)
            .HasConversion(
                t => t.Value,
                value => NotificationType.From(value))
            .HasMaxLength(NotificationType.MaxLength)
            .IsRequired();

        b.Property(n => n.Payload)
            .HasMaxLength(Notification.PayloadMaxLength)
            .IsRequired();

        b.Property(n => n.Status)
            .HasConversion<int>()
            .IsRequired();

        b.Property(n => n.IsRead)
            .HasDefaultValue(false)
            .IsRequired();

        b.Property(n => n.ReadAtUtc);

        b.Property(n => n.CreatedAtUtc).IsRequired();
        b.Property(n => n.UpdatedAtUtc);
        b.Property(n => n.SentAtUtc);

        b.Property(n => n.RetryCount)
            .HasDefaultValue(0)
            .IsRequired();

        b.Property(n => n.NextAttemptAtUtc);
        b.Property(n => n.ExpiresAtUtc);

        b.Property(n => n.IdempotencyKey)
            .HasConversion(
                k => k.HasValue ? k.Value.Value : null,
                value => string.IsNullOrWhiteSpace(value) ? null : IdempotencyKey.From(value))
            .HasMaxLength(IdempotencyKey.MaxLength);

        // --- Indexes
        // Consultation inbox (liste)
        b.HasIndex(n => new { n.UserId, n.CreatedAtUtc });

        // Unread count / filtre read/unread
        b.HasIndex(n => new { n.UserId, n.IsRead });

        // Traitement worker : Pending + échéance
        b.HasIndex(n => new { n.Status, n.NextAttemptAtUtc });

        // Anti-doublon fonctionnel (plusieurs sources)
        b.HasIndex(n => new { n.UserId, n.IdempotencyKey })
            .IsUnique();
    }
}
