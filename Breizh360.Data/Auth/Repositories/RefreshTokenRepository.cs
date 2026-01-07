using Breizh360.Domaine.Auth.RefreshTokens;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Auth.Repositories;

public sealed class RefreshTokenRepository /* : IRefreshTokenRepository */
{
    private readonly Breizh360DbContext _db;

    public RefreshTokenRepository(Breizh360DbContext db) => _db = db;

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => _db.RefreshTokens.FirstOrDefaultAsync(t => EF.Property<string>(t, "TokenHash") == tokenHash, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => await _db.RefreshTokens.AddAsync(token, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
