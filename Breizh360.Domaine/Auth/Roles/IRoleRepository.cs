using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Breizh360.Domaine.Auth.Roles;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Name normalisé conseillé (Trim + ToLowerInvariant) si vous choisissez un nom “case-insensitive”.
    /// </summary>
    Task<Role?> GetByNameAsync(string normalizedName, CancellationToken ct = default);

    Task<IReadOnlyList<Role>> ListAsync(CancellationToken ct = default);

    Task AddAsync(Role role, CancellationToken ct = default);

    Task UpdateAsync(Role role, CancellationToken ct = default);

    Task SoftDeleteAsync(Role role, CancellationToken ct = default);

    /// <summary>
    /// Retourne les rôles d’un user.
    /// </summary>
    Task<IReadOnlyList<Role>> ListForUserAsync(Guid userId, CancellationToken ct = default);
}
