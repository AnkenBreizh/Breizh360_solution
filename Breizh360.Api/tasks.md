# Tâches — API Métier

> **Dernière mise à jour :** 2026-01-10  
> Rappel : **Fini = Remise**. Ne pas marquer “Done” sans lien vers une remise vérifiable.

## Backlog / Suivi

| ID | Sujet | Statut | Dépendances / blocages | Livrable (remise) | DoD (résumé) |
|---|---|---|---|---|---|
| `USR-API-001` | Endpoints `/users` (DTO + erreurs) | **Done** ✅ | Contrat `IF-API-USR-001` (Draft → MVP complété) | `Breizh360.Api/Controllers/UsersController.cs` + `Breizh360.Api/Contracts/Users/*` | GET list + GET by id + DTO + erreurs + doc |
| `NOTIF-API-001` | Hub SignalR + événements | **Done** ✅ | Décision inbox persistée **reportée** (ack no-op) | `Breizh360.Api/Hubs/NotificationsHub.cs` + `Breizh360.Api/Services/NotificationsPublisher.cs` + `Breizh360.Api/Controllers/NotificationsController.cs` + `Breizh360.Api/Contracts/Notifications/*` | Hub + event `notification.received` + endpoint test + doc |

## Notes de pilotage
- Associer chaque tâche à **un contrat `IF-...`** (ou plusieurs si besoin), et inversement.
- Si un point reste flou : ouvrir une demande dans `/Docs/requests.md` (règle : interface non documentée = inexistante).
