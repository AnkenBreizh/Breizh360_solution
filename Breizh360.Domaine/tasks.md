# Tâches — Domaine

> 2026-01-10

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
    - `Breizh360.Domaine/Auth/Users/IAuthUserRepository.cs`
    - `Breizh360.Domaine/Auth/Users/IUserRepository.cs` *(alias Obsolete)*
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

- `NOTIF-DOM-001` — Modèle inbox **persistée** (entités + repository + contrat) — ✅ **Fini (remis)**
  - **Décision :** ✅ Inbox persistée (voir `Docs/decisions/ADR-0002-notif-inbox.md`)
  - **Objectif :** permettre l’historique, l’unread count, l’ack/read, la rejouabilité (si nécessaire).
  - **Contrat :** `IF-NOTIF-001` (déjà publié dans `Breizh360.Domaine/interfaces.md`)
  - **Remise :**
    - `Breizh360.Domaine/interfaces.md` (IF-NOTIF-001)
    - `Breizh360.Domaine/Notifications/Entities/Notification.cs`
    - `Breizh360.Domaine/Notifications/ValueObjects/NotificationId.cs`
    - `Breizh360.Domaine/Notifications/ValueObjects/NotificationType.cs`
    - `Breizh360.Domaine/Notifications/ValueObjects/NotificationStatus.cs`
    - `Breizh360.Domaine/Notifications/ValueObjects/IdempotencyKey.cs`
    - `Breizh360.Domaine/Notifications/Repositories/INotificationRepository.cs`
    - `Breizh360.Domaine/Notifications/Senders/INotificationSender.cs`
  - **Dépendances :** `NOTIF-REQ-003` (contrat domaine final) / `NOTIF-REQ-004` (persistance Data)

