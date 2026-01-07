using Breizh360.Domaine.Auth.Permissions;
using Breizh360.Domaine.Auth.Roles;
using Breizh360.Domaine.Auth.Users;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Data.Auth.Seed;

public static class AuthSeedDev
{
    public static async Task EnsureSeedAsync(Breizh360DbContext db, CancellationToken ct = default)
    {
        // ⚠️ Ici on seed "simple" (noms/keys). Si vos entités imposent des factories/validations,
        // utilisez les méthodes du Domaine (constructeurs/factories) au lieu de new().

        // 1) Permissions (exemples)
        await EnsurePermissionAsync(db, "auth.read", ct);
        await EnsurePermissionAsync(db, "auth.write", ct);

        // 2) Roles (exemples)
        await EnsureRoleAsync(db, "Admin", ct);
        await EnsureRoleAsync(db, "User", ct);

        // 3) Admin user (exemple)
        // ⚠️ Le hash doit respecter la stratégie du Domaine.
        // => si votre Domaine expose "User.Create(...)" ou "PasswordHasher", utilisez-le côté Métier puis persistez.
        // Ici on laisse volontairement un TODO.
        var adminEmail = "admin@local";
        var exists = await db.Users.AnyAsync(u => EF.Property<string>(u, "Email") == adminEmail, ct);
        if (!exists)
        {
            // TODO: construire un User valide selon votre Domaine
            // var admin = User.CreateAdmin(adminEmail, passwordHash, ...);

            // Exemple fallback (à remplacer) :
            var admin = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;
            db.Users.Add(admin);
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task EnsurePermissionAsync(Breizh360DbContext db, string key, CancellationToken ct)
    {
        var exists = await db.Permissions.AnyAsync(p => EF.Property<string>(p, "Key") == key, ct);
        if (exists) return;

        var permission = (Permission)Activator.CreateInstance(typeof(Permission), nonPublic: true)!;
        db.Permissions.Add(permission);
    }

    private static async Task EnsureRoleAsync(Breizh360DbContext db, string name, CancellationToken ct)
    {
        var exists = await db.Roles.AnyAsync(r => EF.Property<string>(r, "Name") == name, ct);
        if (exists) return;

        var role = (Role)Activator.CreateInstance(typeof(Role), nonPublic: true)!;
        db.Roles.Add(role);
    }
}
