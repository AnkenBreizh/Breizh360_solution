using Breizh360.Domaine.Auth.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Auth.Repositories;

public sealed class PermissionRepository /* : IPermissionRepository */
{
    private readonly Breizh360DbContext _db;

    public PermissionRepository(Breizh360DbContext db) => _db = db;

    public Task<Permission?> GetByKeyAsync(string key, CancellationToken ct = default)
        => _db.Permissions.FirstOrDefaultAsync(p => EF.Property<string>(p, "Key") == key, ct);

    public async Task AddAsync(Permission permission, CancellationToken ct = default)
        => await _db.Permissions.AddAsync(permission, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
