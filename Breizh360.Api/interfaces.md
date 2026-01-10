# Interfaces exposées — API Métier

> **Dernière mise à jour :** 2026-01-10  
> **Version des contrats :** 0.1.0 (MVP)  
> **Responsable :** Équipe API  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + REQ + note de migration

## Points à trancher (non bloquants pour le MVP)
- **NOTIF** : Inbox persistée ou non ? (impacte : ack/retry, idempotence, “read/unread”, stockage).  
  ➜ MVP : `Ack(notificationId)` est **no-op** (réservé).

---

## USR (Users)

### `IF-API-USR-001` — Endpoints `/users` (MVP)
- **Responsabilité :** consultation des utilisateurs (read-only).
- **Consommateurs :** UI / Admin (à préciser).
- **Auth :** JWT Bearer, **[Authorize]**.
- **Routes :**
  - `GET /users?page=1&pageSize=20&q=...`
    - `page` : int (≥1)
    - `pageSize` : int (1..100)
    - `q` : string (optionnel) filtre sur login/email (contains, case-insensitive)
  - `GET /users/{id}` (`id` = GUID)
- **Réponses :**
  - `200` :
    - `GET /users` → `UsersContractsUsersResponse` (page, pageSize, totalCount, items[])
    - `GET /users/{id}` → `UsersContractsUserDetailsDto`
  - `400` validation paramètres (format standard)
  - `401/403` auth/authz
  - `404` user not found (format standard)
- **DTO :**
  - `UsersContractsUserSummaryDto` : id, login, email, isActive, createdAt
  - `UsersContractsUserDetailsDto` : id, login, email, isActive, createdAt, updatedAt, roleIds[]
- **Erreurs :** `ErrorsApiError` (cf. middleware + InvalidModelState).
- **Remise :**
  - `Breizh360.Api/Controllers/UsersController.cs`
  - `Breizh360.Api/Contracts/Users/*`

---

## NOTIF (Notifications)

### `IF-API-NOTIF-001` — Hub SignalR + événements (MVP)
- **Responsabilité :** diffusion temps réel de notifications (server → client).
- **Consommateurs :** UI web/mobile (à préciser).
- **Auth :** JWT Bearer (querystring `access_token` supporté pour SignalR) + **[Authorize]**.
- **Hub :** `/hubs/notifications`
- **Événement Server → Client :**
  - Méthode : `notification.received`
  - Payload : `NotificationsContractsNotificationDto` (id, type, message, createdAt, metadata?)
- **Client → Server :**
  - `Ack(notificationId)` : **no-op** (réservé à la future décision “inbox persistée”)
- **Convention d’adressage :**
  - Le client est inscrit au groupe `user:{userId}` lors de la connexion (userId issu du claim NameIdentifier).
- **Endpoint HTTP de test (MVP) :**
  - `POST /notifications/test` (auth requis) — émet une notification vers l’utilisateur courant.
  - Requête : `NotificationsContractsTestNotificationRequest` (type, message, metadata?)
  - Réponse : `202 Accepted` + `NotificationsContractsNotificationDto`
- **Remise :**
  - `Breizh360.Api/Hubs/NotificationsHub.cs`
  - `Breizh360.Api/Services/NotificationsPublisher.cs`
  - `Breizh360.Api/Controllers/NotificationsController.cs`
  - `Breizh360.Api/Contracts/Notifications/*`
