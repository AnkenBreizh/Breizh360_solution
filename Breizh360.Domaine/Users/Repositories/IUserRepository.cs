using Breizh360.Domaine.Users.Entities;
using Breizh360.Domaine.Users.ValueObjects;

namespace Breizh360.Domaine.Users.Repositories;

/// <summary>
/// Contrat de persistance User côté domaine.
/// L'implémentation est attendue dans Breizh360.Data.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);

    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(UserId id, CancellationToken ct = default);
}
