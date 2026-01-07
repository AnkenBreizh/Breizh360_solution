using Breizh360.Domaine.Auth.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class RolePermissionEfConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.ToTable("auth_role_permissions");

        b.HasKey("RoleId", "PermissionId");

        b.HasIndex("RoleId");
        b.HasIndex("PermissionId");
    }
}
