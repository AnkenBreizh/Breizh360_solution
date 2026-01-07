using System;
using Breizh360.Domaine.Common;

namespace Breizh360.Domaine.Auth.Roles;

public sealed class RolePermission
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    private RolePermission() { } // EF

    public RolePermission(Guid roleId, Guid permissionId)
    {
        if (roleId == Guid.Empty) throw new DomainException("RoleId invalide.");
        if (permissionId == Guid.Empty) throw new DomainException("PermissionId invalide.");

        RoleId = roleId;
        PermissionId = permissionId;
    }
}
