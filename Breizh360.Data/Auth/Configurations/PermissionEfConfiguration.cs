using Breizh360.Domaine.Auth.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class PermissionEfConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("auth_permissions");

        // Clé
        b.Property<Guid>("Id");
        b.HasKey("Id");

        // Colonnes
        // NOTE: le contrat Domaine parle de "Code".
        b.Property<string>("Code").HasMaxLength(160).IsRequired();
        b.HasIndex("Code").IsUnique();

        // Audit / soft delete
        b.Property<DateTimeOffset>("CreatedAt");
        b.Property<DateTimeOffset?>("UpdatedAt");
        b.Property<bool>("IsDeleted").HasDefaultValue(false);
        b.Property<DateTimeOffset?>("DeletedAt");
    }
}
