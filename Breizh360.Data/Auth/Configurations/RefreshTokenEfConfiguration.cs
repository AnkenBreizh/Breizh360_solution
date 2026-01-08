using Breizh360.Domaine.Auth.RefreshTokens;
using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class RefreshTokenEfConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("auth_refresh_tokens");

        // Clé
        b.Property<Guid>("Id");
        b.HasKey("Id");

        // FK
        b.Property<Guid>("UserId");
        b.HasIndex("UserId");

        // Token (hash uniquement)
        b.Property<string>("TokenHash").HasMaxLength(256).IsRequired();
        b.HasIndex("TokenHash").IsUnique();

        b.Property<string?>("ReplacedByTokenHash").HasMaxLength(256);

        b.Property<DateTimeOffset>("ExpiresAt");
        b.Property<DateTimeOffset>("CreatedAt");
        b.Property<DateTimeOffset?>("RevokedAt");
        b.Property<string?>("RevokedReason").HasMaxLength(200);

        // Soft delete
        b.Property<bool>("IsDeleted").HasDefaultValue(false);
        b.Property<DateTimeOffset?>("DeletedAt");

        // Relations
        b.HasOne<User>()
            .WithMany()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
