using Breizh360.Domaine.Auth.Permissions;
using Breizh360.Domaine.Auth.RefreshTokens;
using Breizh360.Domaine.Auth.Roles;
using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Auth.Seed;

/// <summary>
/// Seed DEV pour le module Auth : permissions / rôles / admin.
/// ⚠️ Ne pas utiliser tel quel en PROD (identifiants/secret/hash à fournir côté Métier).
/// </summary>
public static class AuthSeedDev
{
    public static async Task EnsureSeedAsync(Breizh360DbContext db, CancellationToken ct = default)
    {
        // 1) Permissions
        var perms = new[]
        {
            "auth.read",
            "auth.write",
            "auth.admin"
        };

        foreach (var p in perms)
            await EnsurePermissionAsync(db, p, ct);

        // 2) Rôles
        await EnsureRoleAsync(db, "Admin", ct);
        await EnsureRoleAsync(db, "User", ct);

        await db.SaveChangesAsync(ct);

        // 3) Liens rôle <-> permissions
        var adminRoleId = await db.Roles
            .Where(r => EF.Property<string>(r, "Name") == "Admin")
            .Select(r => EF.Property<Guid>(r, "Id"))
            .SingleAsync(ct);

        var userRoleId = await db.Roles
            .Where(r => EF.Property<string>(r, "Name") == "User")
            .Select(r => EF.Property<Guid>(r, "Id"))
            .SingleAsync(ct);

        var permIds = await db.Permissions
            .Where(p => perms.Contains(EF.Property<string>(p, "Code")))
            .Select(p => new { Code = EF.Property<string>(p, "Code"), Id = EF.Property<Guid>(p, "Id") })
            .ToListAsync(ct);

        // Admin : toutes les permissions
        foreach (var perm in permIds)
            await EnsureRolePermissionAsync(db, adminRoleId, perm.Id, ct);

        // User : lecture
        var readPermId = permIds.Single(p => p.Code == "auth.read").Id;
        await EnsureRolePermissionAsync(db, userRoleId, readPermId, ct);

        // 4) Admin user
        var adminEmail = "admin@local";
        var adminExists = await db.Users.AnyAsync(u => EF.Property<string>(u, "Email") == adminEmail, ct);
        if (!adminExists)
        {
            // ⚠️ Le Domaine doit décider du hashing (argon2/bcrypt/...)
            // Ici : placeholder DEV. À remplacer côté Métier puis persister.
            var admin = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;

            db.Users.Add(admin);

            // Shadow properties (ou propriétés privées mappées) :
            db.Entry(admin).Property<Guid>("Id").CurrentValue = Guid.NewGuid();
            db.Entry(admin).Property<string>("Email").CurrentValue = adminEmail;
            // Login DEV : aligné sur l'email sans domaine si le champ existe
            if (db.Entry(admin).Metadata.FindProperty("Login") != null)
                db.Entry(admin).Property<string>("Login").CurrentValue = "admin";
            db.Entry(admin).Property<string>("PasswordHash").CurrentValue = "DEV:CHANGE_ME";

            // Associe le rôle Admin
            await db.SaveChangesAsync(ct); // pour s'assurer que l'Id est présent

            var adminId = await db.Users
                .Where(u => EF.Property<string>(u, "Email") == adminEmail)
                .Select(u => EF.Property<Guid>(u, "Id"))
                .SingleAsync(ct);

            await EnsureUserRoleAsync(db, adminId, adminRoleId, ct);
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task EnsurePermissionAsync(Breizh360DbContext db, string code, CancellationToken ct)
    {
        var exists = await db.Permissions.AnyAsync(p => EF.Property<string>(p, "Code") == code, ct);
        if (exists) return;

        var permission = (Permission)Activator.CreateInstance(typeof(Permission), nonPublic: true)!;
        db.Permissions.Add(permission);

        db.Entry(permission).Property<Guid>("Id").CurrentValue = Guid.NewGuid();
        db.Entry(permission).Property<string>("Code").CurrentValue = code;
    }

    private static async Task EnsureRoleAsync(Breizh360DbContext db, string name, CancellationToken ct)
    {
        var exists = await db.Roles.AnyAsync(r => EF.Property<string>(r, "Name") == name, ct);
        if (exists) return;

        var role = (Role)Activator.CreateInstance(typeof(Role), nonPublic: true)!;
        db.Roles.Add(role);

        db.Entry(role).Property<Guid>("Id").CurrentValue = Guid.NewGuid();
        db.Entry(role).Property<string>("Name").CurrentValue = name;
    }

    private static async Task EnsureUserRoleAsync(Breizh360DbContext db, Guid userId, Guid roleId, CancellationToken ct)
    {
        var exists = await db.UserRoles.AnyAsync(ur =>
            EF.Property<Guid>(ur, "UserId") == userId &&
            EF.Property<Guid>(ur, "RoleId") == roleId, ct);

        if (exists) return;

        var link = (UserRole)Activator.CreateInstance(typeof(UserRole), nonPublic: true)!;
        db.UserRoles.Add(link);

        db.Entry(link).Property<Guid>("UserId").CurrentValue = userId;
        db.Entry(link).Property<Guid>("RoleId").CurrentValue = roleId;
    }

    private static async Task EnsureRolePermissionAsync(Breizh360DbContext db, Guid roleId, Guid permissionId, CancellationToken ct)
    {
        var exists = await db.RolePermissions.AnyAsync(rp =>
            EF.Property<Guid>(rp, "RoleId") == roleId &&
            EF.Property<Guid>(rp, "PermissionId") == permissionId, ct);

        if (exists) return;

        var link = (RolePermission)Activator.CreateInstance(typeof(RolePermission), nonPublic: true)!;
        db.RolePermissions.Add(link);

        db.Entry(link).Property<Guid>("RoleId").CurrentValue = roleId;
        db.Entry(link).Property<Guid>("PermissionId").CurrentValue = permissionId;
    }
}
