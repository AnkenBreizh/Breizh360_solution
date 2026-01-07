using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Breizh360.Domaine.Auth.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Recherche par login normalisé (Trim + ToLowerInvariant).
    /// </summary>
    Task<User?> GetByLoginAsync(string normalizedLogin, CancellationToken ct = default);

    /// <summary>
    /// Recherche par email normalisé (Trim + ToLowerInvariant).
    /// </summary>
    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken ct = default);

    Task<bool> ExistsByLoginAsync(string normalizedLogin, CancellationToken ct = default);

    Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken ct = default);

    Task AddAsync(User user, CancellationToken ct = default);

    Task UpdateAsync(User user, CancellationToken ct = default);

    /// <summary>
    /// Soft delete (Doit respecter User.SoftDelete()).
    /// </summary>
    Task SoftDeleteAsync(User user, CancellationToken ct = default);

    /// <summary>
    /// Récupère les rôles (ids) d’un user.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetRoleIdsAsync(Guid userId, CancellationToken ct = default);
}
