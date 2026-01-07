using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Auth.Repositories;

public sealed class UserRepository /* : IUserRepository */
{
    private readonly Breizh360DbContext _db;

    public UserRepository(Breizh360DbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => EF.Property<Guid>(u, "Id") == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => EF.Property<string>(u, "Email") == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
