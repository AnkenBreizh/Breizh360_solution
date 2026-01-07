using Breizh360.Domaine.Auth.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class PermissionEfConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("auth_permissions");

        b.HasKey("Id");

        b.Property("Key").HasMaxLength(160);
        b.HasIndex("Key").IsUnique();

        b.Property("CreatedAt");
        b.Property("UpdatedAt");
        b.Property("IsDeleted");
        b.Property("DeletedAt");
    }
}
