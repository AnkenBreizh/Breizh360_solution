using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class UserRoleEfConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.ToTable("auth_user_roles");

        // Clé composite typique : UserId + RoleId
        b.HasKey("UserId", "RoleId");

        b.HasIndex("UserId");
        b.HasIndex("RoleId");
    }
}
