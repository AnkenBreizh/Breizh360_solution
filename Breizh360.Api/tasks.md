# Tâches — API Métier

> **Dernière mise à jour :** 2026-01-09  
> Rappel : **Fini = Remise**. Ne pas marquer “Done” sans lien vers une remise vérifiable.

## Backlog / Suivi

| ID | Sujet | Statut | Dépendances / blocages | Livrable (remise attendue) | DoD (résumé) |
|---|---|---|---|---|---|
| `AUTH-API-001` | Implémenter `/auth/login` + `/auth/refresh` + `/auth/logout` | In progress | `AUTH-REQ-001`, `AUTH-REQ-002` | `Breizh360.Api/Controllers/AuthController.cs` + services Métier | Login/Refresh/Logout opérationnels + erreurs standard + remise |
| `AUTH-API-002` | Implémenter `GET /me` (claims → `MeContractsMeResponse`) | Backlog | `AUTH-REQ-001` | `Breizh360.Api/Controllers/MeController.cs` | /me renvoie userId/login/email/roles/perms + 401 + remise |
| `USR-API-001` | (Préparer) Endpoints `/users` (DTO + erreurs) | Backlog | `USR-REQ-002` | `Breizh360.Api/Controllers/UsersController.cs` (à créer) | Contrat + DTO + erreurs + doc + remise |
| `NOTIF-API-001` | Hub SignalR `/hubs/notifications` + événements | Backlog | `NOTIF-REQ-001` + décision inbox (`ADR-0002`) | `Breizh360.Api/Hubs/NotificationsHub.cs` + mapping Program.cs | Contrat + méthodes hub + payload + doc + remise |


## Notes de pilotage
- Associer chaque tâche à **un contrat `IF-...`** (ou plusieurs si besoin), et inversement.
- Ajouter un **lien REQ** dans “Dépendances / blocages” dès qu’il y a du flou (cf. `/Docs/requests.md`).