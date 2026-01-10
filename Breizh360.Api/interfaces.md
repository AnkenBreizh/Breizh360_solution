# Interfaces exposées — API Métier

> **Dernière mise à jour :** 2026-01-09  
> **Version des contrats :** 0.1.0 (Draft)  
> **Responsable :** Équipe API  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + REQ + note de migration

## Décisions (transverses)
- **NOTIF** : ✅ Inbox persistée (voir `Docs/decisions/ADR-0002-notif-inbox.md`) → endpoints inbox + ack/read + unread count.
- **USR** : périmètre exact des endpoints `/users` à préciser (pagination/filtre/roles).


> Si un point reste flou : ouvrir une demande dans `/Docs/requests.md` (règle : interface non documentée = inexistante).

---

## USR (Users)

### `IF-API-USR-001` — Endpoints `/users` (Draft)
- **Responsabilité :** exposition des opérations utilisateur définies par le MVP (cf. `USR-API-001`).
- **Consommateurs :** à renseigner (ex: Front, Admin, autres services).
- **Auth :** à préciser (JWT, rôles/scopes).
- **Contrat :** à compléter. Minimum recommandé :
  - `GET /users` (pagination + filtre)
  - `GET /users/{id}`
- **DTO :**
  - `UserSummaryDto` (liste)
  - `UserDetailsDto` (détail)
- **Erreurs :**
  - `400` validation
  - `401/403` authz
  - `404` user not found
  - `500` erreur interne (format standard)
- **Exemple :**
```csharp
// Exemple pseudo-code (à remplacer par DTO réels)
public sealed record UserSummaryDto(Guid Id, string DisplayName);
```
- **Remise :** (chemin + commit/PR) — ex: `Breizh360.Api/Users/...` @ `<ref>`

---

## NOTIF (Notifications)

### `IF-API-NOTIF-001` — Hub SignalR + événements (Draft)
- **Responsabilité :** diffusion temps réel de notifications.
- **Consommateurs :** à renseigner (ex: Front web/mobile).
- **Auth :** à préciser (JWT, claims, groupes).
- **Contrat :** à compléter. Minimum recommandé :
  - Hub : `/hubs/notifications` (chemin à confirmer)
  - Server → Client : `notification.received` (payload typé)
  - Client → Server : `ack(notificationId)` + `markAsRead(notificationId)` (inbox persistée)
- **Payload :**
  - `NotificationDto` (id, type, message, createdAt, metadata)
- **Erreurs / comportements :**
  - Reconnexion, perte de messages, idempotence (selon décision inbox)
- **Exemple :**
```csharp
// Exemple pseudo-code (à remplacer par DTO réels)
public sealed record NotificationDto(Guid Id, string Type, string Message, DateTimeOffset CreatedAt);
```
- **Remise :** (chemin + commit/PR) — ex: `Breizh360.Api/Hubs/...` @ `<ref>`


### `IF-API-NOTIF-002` — Endpoints Inbox (REST) + unread count (Draft)
- **Responsabilité :** exposer l’historique et la synchronisation read/unread.
- **Consommateurs :** UI.
- **Auth :** JWT requis (utilisateur connecté).
- **Routes (proposées) :**
  - `GET /notifications?page=1&pageSize=50` → liste paginée
  - `GET /notifications/unread-count` → compteur
  - `POST /notifications/{id}/read` → marque 1 notification comme lue
  - `POST /notifications/read-all` → marque toutes comme lues *(optionnel)*
- **DTO :**
  - `NotificationDto` (Id, Type, Message, CreatedAt, ReadAt?, Metadata?)
- **Erreurs / comportements :**
  - `404` si notification inexistante ou non accessible
  - idempotence mark-as-read (répéter n’échoue pas)
- **Remise :** `Breizh360.Api/Notifications/...` (controllers + services) + mise à jour contrat

