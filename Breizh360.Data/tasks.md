# Tâches — Données

> **Dernière mise à jour : 08 / 01 / 2026**

## À faire
_Aucun item._

## En cours
- `AUTH-DATA-002` — AUTH-DATA-002 — Dépôts (implémentations)
  - **Détails :** ⚠️ Implémentations à re-valider contre les interfaces Domaine. Erreur remontée : PermissionRepository ne compile pas (CS0535) : IPermissionRepository.GetByIdAsync(Guid,...) , GetByCodeAsync(string,...) , ListAsync(...) manquantes. Remise : Breizh360.Data/Auth/Repositories/*.cs (fix + build vert)

## Fini
- `AUTH-DATA-001` — AUTH-DATA-001 — Schéma + migrations
  - **Détails :** AUTH : schéma PostgreSQL via EF Core + migration initiale (Users/Roles/Permissions + liaisons + RefreshTokens). Soft delete : filtre global + interception SaveChanges . Remise : Breizh360.Data/Breizh360DbContext.cs , Breizh360.Data/Auth/Configurations/*EfConfiguration.cs , Breizh360.Data/Migrations/Auth/20260108080000_AuthInitial.* , Breizh360.Data/Migrations/Auth/Breizh360DbContextModelSnapshot.cs
- `AUTH-DATA-003` — AUTH-DATA-003 — Seed dev
  - **Détails :** Seed dev Auth : permissions auth.read / auth.write / auth.admin , rôles Admin / User , user admin (Login admin , Email admin@local ). Remise : Breizh360.Data/Auth/Seed/AuthSeedDev.cs + doc Breizh360.Data/Auth/README_DEV.md

## Demande
_Aucun item._
