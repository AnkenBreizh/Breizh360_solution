# Demandes inter-équipes — Breizh360

> **Dernière mise à jour :** 2026-01-09  
> **Source :** https://github.com/AnkenBreizh/Breizh360_solution

## Règles
- Une demande = **un livrable**, un **owner**, et une **date cible**.
- Toute demande a un **ID stable** : `XXX-REQ-000`.
- **Done** uniquement si la **remise** est renseignée (chemin/PR/doc).

### Statuts (standard)
- **Backlog** | **Ready** | **In progress** | **Blocked** | **In review** | **Done**

### Priorités (standard)
- **P0** : bloquant critique / prod
- **P1** : bloquant feature en cours
- **P2** : important
- **P3** : nice-to-have

---

## Modèle (copier/coller)
### `XXX-REQ-000` — [TITRE COURT]
- **De :** …
- **À :** …
- **Owner :** … (responsable de suivi côté équipe cible)
- **Priorité :** P0 | P1 | P2 | P3
- **Statut :** Backlog | Ready | In progress | Blocked | In review | Done
- **Nécessaire pour :** `INIT-XXX-000` / Feature (`AUTH`/`USR`/`NOTIF`)
- **Date cible :** …
- **Détails :**
  - …
- **Critères d’acceptation :**
  - …
- **Remise attendue :**
  - `chemin/vers/doc.md` ou PR ou fichier
- **Historique :** (optionnel)
  - 2026-01-09 : création

---

## Demandes — USR (Users)

### `USR-REQ-001` — Contrat Domaine Users (entités + repos)
- **De :** API
- **À :** Domaine
- **Owner :** Domaine
- **Priorité :** P1
- **Statut :** Done
- **Nécessaire pour :** `INIT-USR-001` / `USR`
- **Date cible :** 2026-01-09
- **Détails :**
  - Entités Users (invariants : email unique, normalisation, etc.)
  - Contrats repos (IUserRepository, éventuellement Roles si partagés)
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Domaine/interfaces.md` (sections `IF-USR-…`)
  - Remise = chemins vers fichiers + signatures stables
- **Remise :**
  - `/Breizh360.Domaine/interfaces.md` (IF-USR-001)
  - `/Breizh360.Domaine/Common/DomainException.cs`
  - `/Breizh360.Domaine/Users/Entities/User.cs`
  - `/Breizh360.Domaine/Users/ValueObjects/UserId.cs`
  - `/Breizh360.Domaine/Users/ValueObjects/Email.cs`
  - `/Breizh360.Domaine/Users/ValueObjects/DisplayName.cs`
  - `/Breizh360.Domaine/Users/Repositories/IUserRepository.cs`



### `USR-REQ-002` — Contrat API Users (DTO + erreurs + exemples)
- **De :** UI
- **À :** API
- **Owner :** API
- **Priorité :** P1
- **Statut :** Ready
- **Nécessaire pour :** `INIT-USR-001` / `USR`
- **Date cible :** 2026-01-12
- **Détails :**
  - **Routes / versioning** : base path (ex: `/api/users`) + stratégie de version (si applicable)
  - **Endpoints minimum (UI)** :
    - `GET /users/{id}`
    - `GET /users` (pagination + filtres)
    - `POST /users` (création)
    - `PUT /users/{id}` (édition)
    - `DELETE /users/{id}` (suppression) *(si prévu)*
  - **DTOs** (request/response) : `UserDto`, `CreateUserRequest`, `UpdateUserRequest` (+ champs attendus)
  - **Pagination / filtres** : modèle de réponse paginée, champs obligatoires (page, pageSize, totalCount, items), filtres (email, displayName, tri, recherche)
  - **Erreurs** : mapping HTTP (400 validation, 401, 403, 404, 409 email déjà utilisé, 500) + structure standard du body d’erreur
  - **Exemples** : au moins 1 exemple JSON par endpoint clé (`GET list`, `GET by id`, `POST`, `PUT`) + cas d’erreur 409
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Api/interfaces.md` (sections `IF-API-USR-…`)
  - Exemples utilisables côté UI
- **Remise attendue :**
  - `/Breizh360.Api/interfaces.md` (IF-API-USR-…)
  - *(optionnel mais recommandé)* Swagger/OpenAPI activé (pour vérification rapide côté UI)
  - *(optionnel)* collection Postman / .http Rider
- **Historique :**
  - 2026-01-09 : création
  - 2026-01-09 : complétée (besoins UI pour lever le blocage `USR-UI-001`)

---

## Demandes — NOTIF (Notifications)

### `NOTIF-REQ-001` — Contrat hubs/événements (noms + payloads)
- **De :** UI
- **À :** API
- **Owner :** API
- **Priorité :** P1
- **Statut :** Ready
- **Nécessaire pour :** `INIT-NOTIF-001` / `NOTIF`
- **Date cible :** 2026-01-12
- **Détails :**
  - **Technologie** : SignalR (ou WebSocket brut) + transport(s) autorisés (WebSockets obligatoire ?)
  - **Route publique** : ex. `/hubs/notifications` (et route via Gateway si différente)
  - **Auth** : comment passer le token (Header Bearer vs `access_token` querystring SignalR), claims requis
  - **Événements server -> client** : noms stables + schémas payload (ex: `NotificationReceived`, `UnreadCountChanged`)
  - **Méthodes client -> server** (si nécessaires) : ack, mark-as-read, subscribe/unsubscribe (groupes)
  - **Contrats DTO** : `NotificationDto` + enveloppe éventuelle (type, timestamp, version)
  - **Règles de reconnexion** : comportement attendu (rejoin groupes, replay ?), idempotence
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Api/interfaces.md` (IF-API-NOTIF-…)
  - UI peut implémenter l’abonnement sans ambiguïté
- **Remise attendue :**
  - `/Breizh360.Api/interfaces.md` (IF-API-NOTIF-…)
  - *(optionnel)* exemple de code côté serveur (signature hub) + exemple de client (connexion + handler)
- **Historique :**
  - 2026-01-09 : création
  - 2026-01-09 : complétée (besoins UI pour lever le blocage `NOTIF-UI-001`)

### `NOTIF-REQ-002` — Proxy Gateway `/hubs/*` + WebSockets (validation)
- **De :** API
- **À :** Passerelle
- **Owner :** Passerelle
- **Priorité :** P1
- **Statut :** Ready
- **Nécessaire pour :** `INIT-NOTIF-001` / `NOTIF`
- **Date cible :** 2026-01-12
- **Détails :**
  - **Routage** : `/hubs/*` (ou route exacte) vers API + règles de rewrite (si applicable)
  - **WebSockets** : activé et validé via Gateway (SignalR)
  - **Headers / observabilité** : propagation `X-Correlation-ID`, logs de connexion/déconnexion, codes d’erreur utiles
  - **CORS** : autoriser l’origine UI (dev/prod), méthodes/headers nécessaires
  - **Limites** : timeouts, taille message, keep-alive (valeurs par défaut acceptées ou ajustées)
  - **Validation** : smoke test documenté (commande/URL) + preuve (capture log ou note) que la connexion passe via Gateway
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Gateway/interfaces.md` (IF-GATE-NOTIF-…)
  - Validation smoke test (connexion hub via gateway)
- **Remise attendue :**
  - `/Breizh360.Gateway/interfaces.md` (IF-GATE-NOTIF-…)
  - (si pertinent) extrait config YARP / ReverseProxy (routes + clusters + websocket)
- **Historique :**
  - 2026-01-09 : création
  - 2026-01-09 : complétée (besoins UI pour lever le blocage `NOTIF-UI-001`)


### `NOTIF-REQ-003` — Contrat Domaine Inbox (entités + repository + erreurs)
- **De :** Métier
- **À :** Domaine
- **Owner :** Domaine
- **Priorité :** P1
- **Statut :** Ready
- **Nécessaire pour :** `INIT-NOTIF-001` / `NOTIF`
- **Date cible :** 2026-01-12
- **Détails :**
  - Modèle domaine **inbox persistée** (read/unread, timestamps, type, payload)
  - Repository (ex : `INotificationRepository`) + signatures stables
  - Invariants (idempotence, clés fonctionnelles si besoin)
- **Critères d’acceptation :**
  - Contrat publié/complété dans `Breizh360.Domaine/interfaces.md` (`IF-NOTIF-001`)
  - Remise = chemins des entités + repo + exceptions éventuelles
- **Remise attendue :**
  - `/Breizh360.Domaine/interfaces.md` (IF-NOTIF-001)
  - `/Breizh360.Domaine/Notifications/...`

### `NOTIF-REQ-004` — Persistance EF + migrations Inbox (tables + index + repo)
- **De :** Domaine
- **À :** Données
- **Owner :** Données
- **Priorité :** P1
- **Statut :** Ready
- **Nécessaire pour :** `INIT-NOTIF-001` / `NOTIF`
- **Date cible :** 2026-01-15
- **Détails :**
  - Tables / index / contraintes (unicité, FK, performances)
  - Migrations EF Core + snapshot à jour
  - Implémentation repo EF (conforme au contrat Domaine)
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Data/interfaces.md` (`IF-DATA-NOTIF-001`)
  - Migration reproductible (`dotnet ef migrations script` / apply)
- **Remise attendue :**
  - `/Breizh360.Data/interfaces.md` (IF-DATA-NOTIF-001)
  - `/Breizh360.Data/Notifications/...`
  - `/Breizh360.Data/Migrations/...`

### `NOTIF-REQ-005` — Endpoints Inbox + unread count (REST) + intégration hub (ack/read)
- **De :** UI
- **À :** API
- **Owner :** API
- **Priorité :** P1
- **Statut :** Backlog
- **Nécessaire pour :** `INIT-NOTIF-001` / `NOTIF`
- **Date cible :** 2026-01-19
- **Détails :**
  - Endpoints inbox (liste paginée, unread count, mark-as-read)
  - Compléter le hub avec ack/read si nécessaire
  - Définir erreurs, auth, et comportements (reconnexion)
- **Critères d’acceptation :**
  - Contrats publiés dans `Breizh360.Api/interfaces.md` (`IF-API-NOTIF-001`, `IF-API-NOTIF-002`)
  - UI peut charger l’historique et synchroniser l’unread count
- **Remise attendue :**
  - `/Breizh360.Api/interfaces.md` (IF-API-NOTIF-001, IF-API-NOTIF-002)
  - `/Breizh360.Api/Notifications/...` (controllers/services/hub)
