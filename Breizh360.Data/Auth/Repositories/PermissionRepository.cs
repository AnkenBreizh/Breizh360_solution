using Breizh360.Domaine.Auth.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Auth.Repositories;

/// <summary>
/// Implémentation EF Core du dépôt de permissions.
/// Les propriétés sont accédées via EF.Property("...") pour rester compatible
/// avec des entités Domaine à setters privés/constructeurs non publics.
/// </summary>
public sealed class PermissionRepository : IPermissionRepository
{
    private readonly Breizh360DbContext _db;

    public PermissionRepository(Breizh360DbContext db) => _db = db;

    public Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Permissions.FirstOrDefaultAsync(p => EF.Property<Guid>(p, "Id") == id, ct);

    public Task<Permission?> GetByCodeAsync(string code, CancellationToken ct = default)
        => _db.Permissions.FirstOrDefaultAsync(p => EF.Property<string>(p, "Code") == code, ct);

    public async Task<IReadOnlyList<Permission>> ListAsync(CancellationToken ct = default)
        => await _db.Permissions
            .OrderBy(p => EF.Property<string>(p, "Code"))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Permission>> ListForUserAsync(Guid userId, CancellationToken ct = default)
        => await (
                from ur in _db.UserRoles
                join rp in _db.RolePermissions
                    on EF.Property<Guid>(ur, "RoleId") equals EF.Property<Guid>(rp, "RoleId")
                join p in _db.Permissions
                    on EF.Property<Guid>(rp, "PermissionId") equals EF.Property<Guid>(p, "Id")
                where EF.Property<Guid>(ur, "UserId") == userId
                select p
            )
            .Distinct()
            .OrderBy(p => EF.Property<string>(p, "Code"))
            .ToListAsync(ct);

    public async Task AddAsync(Permission permission, CancellationToken ct = default)
        => await _db.Permissions.AddAsync(permission, ct);

    public Task UpdateAsync(Permission permission, CancellationToken ct = default)
    {
        _db.Permissions.Update(permission);
        return Task.CompletedTask;
    }

    public Task SoftDeleteAsync(Permission permission, CancellationToken ct = default)
    {
        // Le DbContext transforme la suppression en soft delete (IsDeleted/DeletedAt)
        // lors de SaveChanges/SaveChangesAsync.
        _db.Permissions.Remove(permission);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
