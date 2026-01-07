using System;

namespace Breizh360.Domaine.Common;

public abstract class AuditEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    // Audit
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Soft delete
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    protected AuditEntity() { } // EF

    public void MarkCreated(Guid? actorId = null, DateTimeOffset? now = null)
    {
        if (CreatedAt != default) return;
        CreatedAt = now ?? DateTimeOffset.UtcNow;
        CreatedBy = actorId;
    }

    public void MarkUpdated(Guid? actorId = null, DateTimeOffset? now = null)
    {
        UpdatedAt = now ?? DateTimeOffset.UtcNow;
        UpdatedBy = actorId;
    }

    public void SoftDelete(Guid? actorId = null, DateTimeOffset? now = null)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = now ?? DateTimeOffset.UtcNow;
        DeletedBy = actorId;

        MarkUpdated(actorId, DeletedAt);
    }

    public void Restore(Guid? actorId = null, DateTimeOffset? now = null)
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;

        MarkUpdated(actorId, now);
    }
}
