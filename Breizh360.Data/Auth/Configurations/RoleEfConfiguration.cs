using Breizh360.Domaine.Auth.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class RoleEfConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("auth_roles");

        b.HasKey("Id");

        b.Property("Name").HasMaxLength(120);
        b.HasIndex("Name").IsUnique();

        b.Property("CreatedAt");
        b.Property("UpdatedAt");
        b.Property("IsDeleted");
        b.Property("DeletedAt");
    }
}
