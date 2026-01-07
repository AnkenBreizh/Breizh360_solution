using System;
using Breizh360.Domaine.Common;

namespace Breizh360.Domaine.Auth.RefreshTokens;

public sealed class RefreshToken : AuditEntity
{
    public Guid UserId { get; private set; }

    /// <summary>
    /// MAC HMAC du refresh token (jamais le token en clair).
    /// Format: HMACSHA256$kid=&lt;keyId&gt;$&lt;base64url(mac)&gt;
    /// </summary>
    public string TokenHash { get; private set; } = default!;

    public DateTimeOffset ExpiresAt { get; private set; }

    // Révocation
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? RevokedBy { get; private set; }
    public string? RevokeReason { get; private set; }

    // Rotation
    public Guid? ReplacedByTokenId { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsDeleted && !IsRevoked && !IsExpired;

    private RefreshToken() { } // EF

    public RefreshToken(Guid userId, string tokenHash, DateTimeOffset expiresAt)
    {
        if (userId == Guid.Empty) throw new DomainException("UserId invalide.");
        SetTokenHash(tokenHash);

        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new DomainException("ExpiresAt invalide (doit être futur).");

        UserId = userId;
        ExpiresAt = expiresAt;
    }

    public void SetTokenHash(string tokenHash)
    {
        tokenHash = (tokenHash ?? string.Empty).Trim();

        if (tokenHash.Length < 20)
            throw new DomainException("TokenHash invalide (trop court).");

        TokenHash = tokenHash;
    }

    public void Revoke(Guid? actorId = null, string? reason = null, DateTimeOffset? now = null)
    {
        if (IsRevoked) return;

        RevokedAt = now ?? DateTimeOffset.UtcNow;
        RevokedBy = actorId;
        RevokeReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();

        MarkUpdated(actorId, RevokedAt);
    }

    public void ReplaceBy(Guid newTokenId, Guid? actorId = null, DateTimeOffset? now = null)
    {
        if (newTokenId == Guid.Empty) throw new DomainException("NewTokenId invalide.");

        ReplacedByTokenId = newTokenId;
        MarkUpdated(actorId, now);
    }
}
