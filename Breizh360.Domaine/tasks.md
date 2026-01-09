# Tâches — Domaine

> 2026-01-09

## AUTH (Authentification / Autorisation)

- `AUTH-DOM-001` — Modèle de domaine Auth (Users, Roles, Permissions, RefreshTokens, invariants) — ✅ **Fini (remis)**
  - **Remise :**
    - `Breizh360.Domaine/Common/AuditEntity.cs`
    - `Breizh360.Domaine/Common/DomainException.cs`
    - `Breizh360.Domaine/Common/Normalization.cs`
    - `Breizh360.Domaine/Auth/Users/User.cs`
    - `Breizh360.Domaine/Auth/Users/UserRole.cs`
    - `Breizh360.Domaine/Auth/Roles/Role.cs`
    - `Breizh360.Domaine/Auth/Roles/RolePermission.cs`
    - `Breizh360.Domaine/Auth/Permissions/Permission.cs`
    - `Breizh360.Domaine/Auth/RefreshTokens/RefreshToken.cs`

- `AUTH-DOM-002` — Contrats repos Auth — ✅ **Fini (remis)**
  - **Remise :**
    - `Breizh360.Domaine/Auth/Users/IUserRepository.cs`
    - `Breizh360.Domaine/Auth/Roles/IRoleRepository.cs`
    - `Breizh360.Domaine/Auth/Permissions/IPermissionRepository.cs`
    - `Breizh360.Domaine/Auth/RefreshTokens/IRefreshTokenRepository.cs`

## USR (Users)

- `USR-DOM-001` — Entités Users (profil, invariants) — ✅ **Fini (remis)**
  - **Remise :**
    - `Breizh360.Domaine/Users/Entities/User.cs`
    - `Breizh360.Domaine/Users/ValueObjects/UserId.cs`
    - `Breizh360.Domaine/Users/ValueObjects/Email.cs`
    - `Breizh360.Domaine/Users/ValueObjects/DisplayName.cs`
- `USR-DOM-002` — Interfaces repos Users — ✅ **Fini (remis)**
  - **Remise :**
    - `Breizh360.Domaine/Users/Repositories/IUserRepository.cs`

## NOTIF (Notifications)

- `NOTIF-DOM-001` — (optionnel) Modèle inbox / persistance — ⏳ **À décider**
  - Dépendance : décision d’architecture (NOTIF persisté ou non)
  - Si validé : ajout entités + repository NOTIF + contrat `IF-NOTIF-001`
