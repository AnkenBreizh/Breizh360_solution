using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class UserEfConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("auth_users");

        // Clé
        b.Property<Guid>("Id");
        b.HasKey("Id");

        // Colonnes
        b.Property<string>("Login").HasMaxLength(80).IsRequired();
        b.HasIndex("Login").IsUnique();

        b.Property<string>("Email").HasMaxLength(320).IsRequired();
        b.HasIndex("Email").IsUnique();

        // Hash de mot de passe (nom à adapter si nécessaire)
        b.Property<string>("PasswordHash").HasMaxLength(512).IsRequired();

        // Audit / soft delete
        b.Property<DateTimeOffset>("CreatedAt");
        b.Property<DateTimeOffset?>("UpdatedAt");

        // ActorId (référence logique, type Guid attendu côté Domaine)
        b.Property<Guid?>("CreatedBy");
        b.Property<Guid?>("UpdatedBy");

        b.Property<bool>("IsDeleted").HasDefaultValue(false);
        b.Property<DateTimeOffset?>("DeletedAt");
        b.Property<Guid?>("DeletedBy");
    }
}
