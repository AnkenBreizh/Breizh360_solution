using Breizh360.Domaine.Auth.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class RoleEfConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("auth_roles");

        // Clé
        b.Property<Guid>("Id");
        b.HasKey("Id");

        // Colonnes
        b.Property<string>("Name").HasMaxLength(120).IsRequired();
        b.HasIndex("Name").IsUnique();

        // Audit / soft delete
        b.Property<DateTimeOffset>("CreatedAt");
        b.Property<DateTimeOffset?>("UpdatedAt");
        b.Property<bool>("IsDeleted").HasDefaultValue(false);
        b.Property<DateTimeOffset?>("DeletedAt");
    }
}
