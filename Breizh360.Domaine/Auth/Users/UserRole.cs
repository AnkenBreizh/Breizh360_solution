using System;
using Breizh360.Domaine.Common;

namespace Breizh360.Domaine.Auth.Users;

public sealed class UserRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    private UserRole() { } // EF

    public UserRole(Guid userId, Guid roleId)
    {
        if (userId == Guid.Empty) throw new DomainException("UserId invalide.");
        if (roleId == Guid.Empty) throw new DomainException("RoleId invalide.");

        UserId = userId;
        RoleId = roleId;
    }
}
