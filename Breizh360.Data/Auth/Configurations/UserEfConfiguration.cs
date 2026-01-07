using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class UserEfConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("auth_users");

        b.HasKey("Id");

        b.Property("Email").HasMaxLength(320);
        b.HasIndex("Email").IsUnique();

        // Hash de mot de passe (nom à adapter si nécessaire)
        b.Property("PasswordHash").HasMaxLength(512);

        // Audit / soft delete (si présent dans le Domaine)
        b.Property("CreatedAt");
        b.Property("UpdatedAt");
        b.Property("CreatedBy");
        b.Property("UpdatedBy");
        b.Property("IsDeleted");
        b.Property("DeletedAt");
        b.Property("DeletedBy");
    }
}
