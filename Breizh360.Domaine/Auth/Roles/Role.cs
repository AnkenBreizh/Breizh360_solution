using Breizh360.Domaine.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Breizh360.Domaine.Auth.Roles;

public sealed class Role : AuditEntity
{
    public const int NameMaxLength = 64;

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    private readonly List<RolePermission> _permissions = new();
    public IReadOnlyCollection<RolePermission> Permissions => _permissions;

    private Role() { } // EF

    public Role(string name, string? description = null)
    {
        SetName(name);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void SetName(string name)
    {
        var normalized = Normalization.NormalizeIdentityKey(name);

        if (normalized.Length is < 2 or > NameMaxLength)
            throw new DomainException($"Role.Name invalide (2..{NameMaxLength}).");

        Name = normalized;
    }

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void AddPermission(Guid permissionId)
    {
        if (permissionId == Guid.Empty) throw new DomainException("PermissionId invalide.");

        if (_permissions.Any(p => p.PermissionId == permissionId))
            return;

        _permissions.Add(new RolePermission(Id, permissionId));
    }

    public void RemovePermission(Guid permissionId)
    {
        _permissions.RemoveAll(p => p.PermissionId == permissionId);
    }
}
