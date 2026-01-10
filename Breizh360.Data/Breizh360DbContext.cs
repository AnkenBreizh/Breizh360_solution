using System.Reflection;
using Breizh360.Data.Common;
using Breizh360.Domaine.Auth.Permissions;
using Breizh360.Domaine.Auth.RefreshTokens;
using Breizh360.Domaine.Auth.Roles;
using Breizh360.Domaine.Auth.Users;
using Breizh360.Domaine.Notifications.Entities;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data;

public sealed class Breizh360DbContext : DbContext
{
    public Breizh360DbContext(DbContextOptions<Breizh360DbContext> options) : base(options) { }

    // Auth
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Notifications (Inbox persistée)
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applique toutes les configurations EF du projet Data
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Filtre global soft delete : si l’entité a un bool IsDeleted => exclure les deleted.
        ApplySoftDeleteQueryFilter(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditAndSoftDelete()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            // Soft delete : transforme "Deleted" en "Modified" + set IsDeleted/DeletedAt
            if (entry.State == EntityState.Deleted && entry.HasProperty("IsDeleted"))
            {
                entry.State = EntityState.Modified;
                entry.SetCurrentValue("IsDeleted", true);
                entry.TrySetCurrentValue("DeletedAt", now);
                // DeletedBy : volontairement non alimenté ici (à faire côté API/Métier si besoin)
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.TrySetCurrentValue("CreatedAt", now);
                entry.TrySetCurrentValue("UpdatedAt", now);
                entry.TrySetCurrentValue("IsDeleted", false);
            }

            if (entry.State == EntityState.Modified)
            {
                entry.TrySetCurrentValue("UpdatedAt", now);
            }
        }
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Si la propriété IsDeleted existe, appliquer un query filter :
            // EF.Property<bool>(e, "IsDeleted") == false
            var isDeletedProp = entityType.FindProperty("IsDeleted");
            if (isDeletedProp is null || isDeletedProp.ClrType != typeof(bool))
                continue;

            var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
            var propMethod = typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(bool));
            var isDeleted = System.Linq.Expressions.Expression.Call(propMethod, parameter, System.Linq.Expressions.Expression.Constant("IsDeleted"));
            var filter = System.Linq.Expressions.Expression.Lambda(
                System.Linq.Expressions.Expression.Equal(isDeleted, System.Linq.Expressions.Expression.Constant(false)),
                parameter
            );

            entityType.SetQueryFilter(filter);
        }
    }
}
