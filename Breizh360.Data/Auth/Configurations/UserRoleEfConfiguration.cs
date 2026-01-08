using Breizh360.Domaine.Auth.Roles;
using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breizh360.Data.Auth.Configurations;

public sealed class UserRoleEfConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.ToTable("auth_user_roles");

        b.Property<Guid>("UserId");
        b.Property<Guid>("RoleId");

        // Clé composite : UserId + RoleId
        b.HasKey("UserId", "RoleId");

        b.HasIndex("UserId");
        b.HasIndex("RoleId");

        b.HasOne<User>()
            .WithMany()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<Role>()
            .WithMany()
            .HasForeignKey("RoleId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
