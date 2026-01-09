# Interfaces exposées — API Métier

> **Dernière mise à jour :** 2026-01-09  
> **Version des contrats :** 0.1.0 (Draft)  
> **Responsable :** Équipe API  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + REQ + note de migration

## Points à trancher (bloquants)
- **AUTH** : rotation refresh + invalidation logout + durée tokens (impacte UI/Tests)
- **NOTIF** : Inbox persistée ou non ? (impacte : ack/retry, idempotence, “read/unread”, stockage)
- **USR** : Périmètre exact des endpoints `/users` (non exposés pour l’instant côté API)

> Si un point reste flou : ouvrir une demande dans `/Docs/requests.md` (règle : interface non documentée = inexistante).

---

## AUTH (Authentification / Autorisation)

### `IF-API-AUTH-001` — Endpoints `/auth/*` (Draft)
- **Routes :**
  - `POST /auth/login` → `AuthContractsTokenPairResponse`
  - `POST /auth/refresh` → `AuthContractsTokenPairResponse`
  - `POST /auth/logout` → `204 NoContent` (ou `200` selon stratégie)
- **DTO (actuel) :**
  - `AuthContractsLoginRequest` : `loginOrEmail`, `password`
  - `AuthContractsRefreshRequest` : `refreshToken`
  - `AuthContractsTokenPairResponse` : `tokenType`, `accessToken`, `expiresInSeconds`, `refreshToken`
- **Erreurs (format `ErrorsApiError`) :**
  - `400` : validation (required/minlength)
  - `401` : identifiants invalides / refresh invalide/expiré
  - `500` : erreur interne
- **Notes :**
  - Les endpoints existent mais retournent actuellement `501 Not Implemented` (cf. TODO `AUTH-API-001`).
  - Décision à documenter : rotation refresh, invalidation logout, durée de vie tokens.
- **Remise :** `Breizh360.Api/Controllers/AuthController.cs` + `Breizh360.Api/Contracts/Auth/*`

### `IF-API-ME-001` — Endpoint `/me` (Draft)
- **Route :** `GET /me` → `MeContractsMeResponse`
- **Auth :** `Authorization: Bearer <accessToken>` (JWT)
- **DTO (actuel) :**
  - `MeContractsMeResponse` : `userId`, `login`, `email`, `roles[]`, `permissions[]`
- **Erreurs (format `ErrorsApiError`) :**
  - `401` : token manquant/invalide
  - `500` : erreur interne
- **Notes :**
  - Retourne actuellement `501 Not Implemented` (cf. TODO `AUTH-API-002`).
  - Source de vérité des claims : à préciser (role/perm, mapping userId/login/email).
- **Remise :** `Breizh360.Api/Controllers/MeController.cs` + `Breizh360.Api/Contracts/Me/*`

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
