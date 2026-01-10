# Suivi — Équipe Métier (Breizh360.Metier)

> **Dernière mise à jour :** 2026-01-10  
> **Règles :** statuts standard + **Done = Remise** (voir `Docs/rules.md`)

## État global

### AUTH
- Le module Auth est **présent** dans `Breizh360.Metier/Auth`.
- Les dépendances aux repositories sont désormais stabilisées via des interfaces explicites
  (`IAuthUserRepository`). L’ancienne implémentation utilisant la réflexion a été
  remplacée (voir `AUTH-MET-003`).

### USR
- Use-cases à démarrer après stabilisation des contrats (notamment “liste/pagination”) et
  disponibilité côté Data.

### NOTIF
- ✅ Inbox persistée confirmée (voir `Docs/decisions/ADR-0002-notif-inbox.md`) : activer
  use-cases + contrat `IF-MET-NOTIF-001` et dépendances Data/API.

## Backlog détaillé

| ID | Feature | Sujet | Statut | Dépendances | Remise (quand Done) | Notes |
|---:|---|:---:|---|---|---|---|
| **MET-DOC-001** | META | Mettre à niveau les fichiers de suivi (README / tasks / interfaces) | **Done** | — | `Breizh360.Metier/README.md`, `Breizh360.Metier/tasks.md`, `Breizh360.Metier/interfaces.md` | Harmonisation IDs + statuts + remises |
| **AUTH-MET-001** | AUTH | Use-cases Auth : validation identifiants + émission tokens | **Done** | Contrats repos Auth (Domaine) | `Breizh360.Metier/Auth/AuthServiceValidateCredentials.cs`, `Breizh360.Api/Controllers/AuthController.cs`, `Breizh360.Api/Services/ApiTokenService.cs` | L’API expose désormais les endpoints Login/Refresh/Logout en appelant le service métier. |
| **AUTH-MET-002** | AUTH | Use-case Autorisation : vérification permission | **In review** | Contrat repo permissions (Domaine) | `Breizh360.Metier/Auth/AuthorizationServiceIsAllowed.cs`, `Breizh360.Metier/Auth/AuthorizationContext.cs` | |
| **AUTH-MET-003** | AUTH | Stabiliser les dépendances repos (supprimer `object`/`dynamic`/reflection) | **Done** | Contrats Domaine clarifiés | `Breizh360.Metier/Auth/AuthServiceValidateCredentials.cs` | Remplacement de `RepoInvoke.CallFirstAsync` par des appels typés au dépôt `IAuthUserRepository`. |
| **USR-MET-001** | USR | Use-cases Users : list/get/update | **Blocked** | Contrats Domaine/Data manquants pour “liste/pagination” | (à définir) | Créer une demande si besoin (contrat de listing + pagination) |
| **NOTIF-MET-001** | NOTIF | Use-cases Notifications (inbox persistée si confirmée) | **Backlog** | Décision inbox + persistance Data + contrats API | (à définir) | À aligner avec `Docs/feature_notif_b2b.md` |

> **Convention** : si un item passe en **Blocked**, référencer une demande (`XXX-REQ-...`) dans la colonne Dépendances.