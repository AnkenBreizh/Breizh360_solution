# Interfaces exposées — API Métier

> **Dernière mise à jour :** 2026-01-09  
> **Version des contrats :** 0.1.0 (Draft)  
> **Responsable :** Équipe API  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + REQ + note de migration

## Points à trancher (bloquants)
- **NOTIF** : Inbox persistée ou non ? (impacte : ack/retry, idempotence, “read/unread”, stockage)
- **USR** : Périmètre exact des endpoints `/users` (read-only ? admin ? pagination/filtre ?)

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
  - (Optionnel) Client → Server : `ack(notificationId)` si inbox persistée
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
