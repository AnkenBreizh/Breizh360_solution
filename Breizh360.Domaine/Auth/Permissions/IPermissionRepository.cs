using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Breizh360.Domaine.Auth.Permissions;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Code stable (ex: users.read).
    /// </summary>
    Task<Permission?> GetByCodeAsync(string code, CancellationToken ct = default);

    Task<IReadOnlyList<Permission>> ListAsync(CancellationToken ct = default);

    Task AddAsync(Permission permission, CancellationToken ct = default);

    Task UpdateAsync(Permission permission, CancellationToken ct = default);

    Task SoftDeleteAsync(Permission permission, CancellationToken ct = default);

    /// <summary>
    /// Permissions d’un user (via rôles).
    /// </summary>
    Task<IReadOnlyList<Permission>> ListForUserAsync(Guid userId, CancellationToken ct = default);
}
