using System;
using System.Threading;
using System.Threading.Tasks;

namespace Breizh360.Domaine.Auth.RefreshTokens;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Recherche par hash (jamais de token en clair).
    /// </summary>
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

    Task AddAsync(RefreshToken token, CancellationToken ct = default);

    Task UpdateAsync(RefreshToken token, CancellationToken ct = default);

    /// <summary>
    /// Révocation en masse (ex: logout partout).
    /// </summary>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);
}
