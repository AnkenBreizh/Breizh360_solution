using Breizh360.Domaine.Auth.Permissions;
using Breizh360.Domaine.Auth.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class RolePermissionEfConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.ToTable("auth_role_permissions");

        b.Property<Guid>("RoleId");
        b.Property<Guid>("PermissionId");

        b.HasKey("RoleId", "PermissionId");

        b.HasIndex("RoleId");
        b.HasIndex("PermissionId");

        b.HasOne<Role>()
            .WithMany()
            .HasForeignKey("RoleId")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<Permission>()
            .WithMany()
            .HasForeignKey("PermissionId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
