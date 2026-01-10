using System;

namespace Breizh360.Domaine.Auth.Users;

/// <summary>
/// ⚠️ Alias historique conservé pour compatibilité.
/// <para>Utiliser <see cref="IAuthUserRepository"/> à la place.</para>
/// </summary>
[Obsolete("Use IAuthUserRepository instead (AUTH).", false)]
public interface IUserRepository : IAuthUserRepository
{
}
