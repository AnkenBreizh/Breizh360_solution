using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Breizh360.Domaine.Auth.Users;

namespace Breizh360.Data.Auth.Repositories;

/// <summary>
/// Implémentation EF Core des repositories utilisateurs. Cette version implémente à la fois
/// <see cref="IAuthUserRepository"/> et l’alias historique <see cref="IUserRepository"/>
/// afin de lever les problèmes de cast rencontrés lors de l’injection (cf. erreur CS0266).
/// </summary>
public sealed class UserRepository : IAuthUserRepository, IUserRepository
{
    private readonly Breizh360DbContext _db;

    public UserRepository(Breizh360DbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => EF.Property<Guid>(u, "Id") == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => EF.Property<string>(u, "Email") == email, ct);

    public Task<User?> GetByLoginAsync(string login, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => EF.Property<string>(u, "Login") == login, ct);

    public Task<bool> ExistsByLoginAsync(string login, CancellationToken ct = default)
        => _db.Users.AnyAsync(u => EF.Property<string>(u, "Login") == login, ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.AnyAsync(u => EF.Property<string>(u, "Email") == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task SoftDeleteAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Guid>> GetRoleIdsAsync(Guid userId, CancellationToken ct = default)
        => await _db.UserRoles
            .Where(ur => EF.Property<Guid>(ur, "UserId") == userId)
            .Select(ur => EF.Property<Guid>(ur, "RoleId"))
            .Distinct()
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}