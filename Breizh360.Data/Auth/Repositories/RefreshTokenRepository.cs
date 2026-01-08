using Breizh360.Domaine.Auth.RefreshTokens;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Auth.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly Breizh360DbContext _db;

    public RefreshTokenRepository(Breizh360DbContext db) => _db = db;

    public Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.RefreshTokens.FirstOrDefaultAsync(t => EF.Property<Guid>(t, "Id") == id, ct);

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => _db.RefreshTokens.FirstOrDefaultAsync(t => EF.Property<string>(t, "TokenHash") == tokenHash, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => await _db.RefreshTokens.AddAsync(token, ct);

    public Task UpdateAsync(RefreshToken token, CancellationToken ct = default)
    {
        _db.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;

        var tokens = await _db.RefreshTokens
            .Where(t => EF.Property<Guid>(t, "UserId") == userId && EF.Property<DateTimeOffset?>(t, "RevokedAt") == null)
            .ToListAsync(ct);

        foreach (var t in tokens)
        {
            _db.Entry(t).Property<DateTimeOffset?>("RevokedAt").CurrentValue = now;
            // champ optionnel : le set reste sans effet si la colonne/propriété n'existe pas
            if (_db.Entry(t).Metadata.FindProperty("RevokedReason") != null)
                _db.Entry(t).Property<string?>("RevokedReason").CurrentValue = "revoke_all";
        }
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
