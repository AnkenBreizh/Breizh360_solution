using Breizh360.Domaine.Auth.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class RefreshTokenEfConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("auth_refresh_tokens");

        b.HasKey("Id");

        b.HasIndex("UserId");
        b.HasIndex("TokenHash").IsUnique();

        b.Property("TokenHash").HasMaxLength(256); // ex SHA-256 hex/base64 selon votre choix
        b.Property("ReplacedByTokenHash").HasMaxLength(256);

        b.Property("ExpiresAt");
        b.Property("CreatedAt");
        b.Property("RevokedAt");
        b.Property("RevokedReason").HasMaxLength(200);

        b.Property("IsDeleted");
        b.Property("DeletedAt");
    }
}
