using Breizh360.Domaine.Auth.Roles;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Auth.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly Breizh360DbContext _db;

    public RoleRepository(Breizh360DbContext db) => _db = db;

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Roles.FirstOrDefaultAsync(r => EF.Property<Guid>(r, "Id") == id, ct);

    public Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
        => _db.Roles.FirstOrDefaultAsync(r => EF.Property<string>(r, "Name") == name, ct);

    public async Task<IReadOnlyList<Role>> ListAsync(CancellationToken ct = default)
        => await _db.Roles
            .OrderBy(r => EF.Property<string>(r, "Name"))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Role>> ListForUserAsync(Guid userId, CancellationToken ct = default)
        => await (
                from ur in _db.UserRoles
                join r in _db.Roles
                    on EF.Property<Guid>(ur, "RoleId") equals EF.Property<Guid>(r, "Id")
                where EF.Property<Guid>(ur, "UserId") == userId
                select r
            )
            .Distinct()
            .OrderBy(r => EF.Property<string>(r, "Name"))
            .ToListAsync(ct);

    public async Task AddAsync(Role role, CancellationToken ct = default)
        => await _db.Roles.AddAsync(role, ct);

    public Task UpdateAsync(Role role, CancellationToken ct = default)
    {
        _db.Roles.Update(role);
        return Task.CompletedTask;
    }

    public Task SoftDeleteAsync(Role role, CancellationToken ct = default)
    {
        _db.Roles.Remove(role);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
