# Interfaces exposées — Données

> **Dernière mise à jour : 08 / 01 / 2026**


## IF-DATA-AUTH-001 — Schéma Auth (EF Core : tables + index + soft delete)
- **Consommateurs :** API Métier, Tests
- **Contrat / exemples :**

Contrat : tables auth_users , auth_roles , auth_permissions , liaisons auth_user_roles , auth_role_permissions , auth_refresh_tokens . Index uniques : User.Login, User.Email, Role.Name, Permission.Code, RefreshToken.TokenHash. Soft delete : filtre global si propriété IsDeleted + interception SaveChanges (Delete => IsDeleted=true ). Exemple : générer les migrations : dotnet ef migrations add Auth_Init -p Breizh360.Data -s Breizh360.Api.Metier puis dotnet ef database update (adapter le projet de startup).

- **Remise :** Breizh360.Data/Breizh360DbContext.cs , Breizh360.Data/Auth/Configurations/*.cs , Breizh360.Data/Migrations/Auth/*

## IF-DATA-AUTH-002 — Seed DEV Auth (admin + rôles + permissions)
- **Consommateurs :** API Métier, Tests
- **Contrat / exemples :**

Contrat : appeler AuthSeedDev.EnsureSeedAsync(db) au démarrage en environnement DEV. Exemple : dans l’API, après migration : await AuthSeedDev.EnsureSeedAsync(db); Erreurs : le seed doit respecter les invariants du Domaine (éviter les Activator.CreateInstance si le Domaine expose des factories).

- **Remise :** Breizh360.Data/Auth/Seed/AuthSeedDev.cs
