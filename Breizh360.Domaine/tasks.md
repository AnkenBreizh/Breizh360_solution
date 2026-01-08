# Tâches — Domaine

> **Dernière mise à jour : 08 / 01 / 2026**

## À faire
_Aucun item._

## En cours
_Aucun item._

## Fini
- `AUTH-DOM-005` — AUTH-DOM-005 — Socle Domaine (audit + soft delete + exceptions + normalisation)
  - **Détails :** Base commune : audit/soft delete, exception de domaine, normalisation des clés d’identité (login/email). Remise : Breizh360.Domaine/Common/AuditEntity.cs , Breizh360.Domaine/Common/DomainException.cs , Breizh360.Domaine/Common/Normalization.cs
- `AUTH-DOM-001` — AUTH-DOM-001 — Entité User
  - **Détails :** Champs, invariants, hash (mot de passe), relations User↔Role. Remise : Breizh360.Domaine/Auth/Users/User.cs , Breizh360.Domaine/Auth/Users/UserRole.cs + doc Breizh360.Domaine/01_modele_domaine.md
- `AUTH-DOM-002` — AUTH-DOM-002 — Role / Permission
  - **Détails :** RBAC/ABAC : entités Role/Permission + liaison Role↔Permission. Remise : Breizh360.Domaine/Auth/Roles/Role.cs , Breizh360.Domaine/Auth/Roles/RolePermission.cs , Breizh360.Domaine/Auth/Permissions/Permission.cs + doc Breizh360.Domaine/01_modele_domaine.md
- `AUTH-DOM-004` — AUTH-DOM-004 — Entité RefreshToken
  - **Détails :** Rotation/revocation : token stocké hashé (jamais en clair) + lien User. Remise : Breizh360.Domaine/Auth/RefreshTokens/RefreshToken.cs + doc Breizh360.Domaine/01_modele_domaine.md
- `AUTH-DOM-003` — AUTH-DOM-003 — Contrats de dépôts
  - **Détails :** Interfaces minimalistes (repositories) attendues par Data/Métier. Remise : Breizh360.Domaine/Auth/Users/IUserRepository.cs , Breizh360.Domaine/Auth/Roles/IRoleRepository.cs , Breizh360.Domaine/Auth/Permissions/IPermissionRepository.cs , Breizh360.Domaine/Auth/RefreshTokens/IRefreshTokenRepository.cs

## Demande
_Aucun item._
