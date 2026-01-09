# Domaine — Breizh360

> 2026-01-09

## Rôle

Le projet **Breizh360.Domaine** contient :
- Le **modèle de domaine** (entités, value objects, invariants)
- Les **contrats de persistance** côté domaine (interfaces de repositories)

## Structure (attendue)

- `Breizh360.Domaine/Common/...` (exceptions, audit, normalisation)
- `Breizh360.Domaine/Auth/...` (AUTH)
- `Breizh360.Domaine/Users/...` (USR)
- `Breizh360.Domaine/Notifications/...` (NOTIF — si décidé)

## Statut

- ✅ `AUTH-DOM-001` — Modèle Auth (Users/Roles/Permissions/RefreshTokens) — **remis**
- ✅ `AUTH-DOM-002` — Repositories Auth — **remis**
- ✅ `USR-DOM-001` — Entités Users (profil, invariants) — **remis**
- ✅ `USR-DOM-002` — Interfaces repos Users — **remis**
- ⏳ `NOTIF` — (optionnel) en attente de décision (persistance inbox)

## Conventions d’équipe

- **ID stable obligatoire** (tasks + interfaces).
- **Contrats avant implémentation** : le fichier `interfaces.md` fait foi.
- **Fini = Remise** : une tâche n’est “done” que si les fichiers attendus sont présents.

## Remise (fichiers livrés)

### Common
- `Breizh360.Domaine/Common/AuditEntity.cs`
- `Breizh360.Domaine/Common/DomainException.cs`
- `Breizh360.Domaine/Common/Normalization.cs`

### AUTH
- `Breizh360.Domaine/Auth/Users/User.cs`
- `Breizh360.Domaine/Auth/Users/UserRole.cs`
- `Breizh360.Domaine/Auth/Users/IUserRepository.cs`
- `Breizh360.Domaine/Auth/Roles/Role.cs`
- `Breizh360.Domaine/Auth/Roles/RolePermission.cs`
- `Breizh360.Domaine/Auth/Roles/IRoleRepository.cs`
- `Breizh360.Domaine/Auth/Permissions/Permission.cs`
- `Breizh360.Domaine/Auth/Permissions/IPermissionRepository.cs`
- `Breizh360.Domaine/Auth/RefreshTokens/RefreshToken.cs`
- `Breizh360.Domaine/Auth/RefreshTokens/IRefreshTokenRepository.cs`

### USR
- `Breizh360.Domaine/Users/Entities/User.cs`
- `Breizh360.Domaine/Users/ValueObjects/UserId.cs`
- `Breizh360.Domaine/Users/ValueObjects/Email.cs`
- `Breizh360.Domaine/Users/ValueObjects/DisplayName.cs`
- `Breizh360.Domaine/Users/Repositories/IUserRepository.cs`
