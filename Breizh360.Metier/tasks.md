# Suivi — Équipe Métier (Breizh360.Metier)

> **Dernière mise à jour :** 2026-01-10  
> **Règles :** statuts standard + **Done = Remise** (voir `Docs/rules.md`)

## État global

### AUTH
- Le module Auth est **présent** dans `Breizh360.Metier/Auth`.
- Les dépendances aux repositories sont **typées** :
  - `IAuthUserRepository` (credentials)
  - `IRefreshTokenRepository` (refresh tokens hashés + rotation)
  - `IPermissionRepository` (RBAC/permissions)
- L’ancienne implémentation “best-effort” par reflection a été supprimée (voir `AUTH-MET-003`).

### USR
- Use-cases à démarrer après stabilisation des contrats (notamment “liste/pagination”) et
  disponibilité côté Data.
- Une demande dédiée a été ouverte : `USR-REQ-003`.

### NOTIF
- ✅ Inbox persistée confirmée (voir `Docs/decisions/ADR-0002-notif-inbox.md`).
- Use-cases Métier à démarrer après alignement contrat API inbox (liste/unread/mark-read) et dépendances Data.

## Backlog détaillé

| ID | Feature | Sujet | Statut | Dépendances | Remise (quand Done) | Notes |
|---:|---|:---:|---|---|---|---|
| **MET-DOC-001** | META | Mettre à niveau les fichiers de suivi (README / tasks / interfaces) | **Done** | — | `Breizh360.Metier/README.md`, `Breizh360.Metier/tasks.md`, `Breizh360.Metier/interfaces.md` | Harmonisation IDs + statuts + remises |
| **AUTH-MET-001** | AUTH | Use-cases Auth : validation identifiants + émission tokens | **Done** | Contrats repos Auth (Domaine) | `Breizh360.Metier/Auth/AuthServiceValidateCredentials.cs` | Credentials via `IAuthUserRepository` |
| **AUTH-MET-002** | AUTH | Use-case Autorisation : vérification permission | **Done** | Contrat repo permissions (Domaine) | `Breizh360.Metier/Auth/AuthorizationServiceIsAllowed.cs`, `Breizh360.Metier/Auth/AuthorizationContext.cs` | RBAC (permissions via rôles) + wildcard suffixe + ABAC minimal `:own` |
| **AUTH-MET-003** | AUTH | Stabiliser les dépendances repos (supprimer reflection) | **Done** | Contrats Domaine clarifiés | `Breizh360.Metier/Auth/AuthorizationServiceIsAllowed.cs`, `Breizh360.Metier/Auth/TokenService.cs` | Suppression `RepoInvoke` (reflection). Les constructeurs `object` restants sont marqués `Obsolete` (compatibilité transitoire). |
| **AUTH-MET-004** | AUTH | Refresh tokens : stockage hashé + rotation | **Done** | `IRefreshTokenRepository` (Domaine) | `Breizh360.Metier/Auth/TokenService.cs`, `Breizh360.Metier/Auth/RefreshTokenCrypto.cs` | `IssueAsync` stocke le hash (si repo injecté). `RefreshAsync` valide + révoque + rotation. |
| **USR-MET-001** | USR | Use-cases Users : list/get/update | **Blocked** | `USR-REQ-003` | (à définir) | Contrat listing/pagination requis (query + tri + filtres) |
| **NOTIF-MET-001** | NOTIF | Use-cases Notifications (inbox persistée) | **Backlog** | `NOTIF-REQ-006` + `NOTIF-REQ-005` | (à définir) | Démarrer quand le contrat API inbox est figé (liste + unread count + mark-read) |

> **Convention** : si un item passe en **Blocked**, référencer une demande (`XXX-REQ-...`) dans la colonne Dépendances.
