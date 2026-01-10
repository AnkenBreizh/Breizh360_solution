# Tâches — API Métier

> **Dernière mise à jour :** 2026-01-09  
> Rappel : **Fini = Remise**. Ne pas marquer “Done” sans lien vers une remise vérifiable.

## Backlog / Suivi

| ID | Sujet | Statut | Dépendances / blocages | Livrable (remise attendue) | DoD (résumé) |
|---|---|---|---|---|---|
| `USR-API-001` | Endpoints `/users` (DTO + erreurs) | Backlog | `USR-REQ-002` + `INIT-AUTH-001` | `Breizh360.Api/Users/...` | Contrat + DTO + erreurs + doc + remise |
| `NOTIF-API-001` | Hub SignalR + événements | Backlog | `NOTIF-REQ-001` + `NOTIF-REQ-002` + `INIT-AUTH-001` | `Breizh360.Api/Hubs/...` | Contrat + hub + payload + doc + remise |
| `NOTIF-API-002` | Endpoints inbox + unread count | Backlog | `NOTIF-REQ-003` + `NOTIF-REQ-004` + `NOTIF-REQ-005` | `Breizh360.Api/Notifications/...` | Contrat REST + DTO + erreurs + doc + remise |

## Notes de pilotage
- Associer chaque tâche à **un contrat `IF-...`** (ou plusieurs si besoin), et inversement.
- Ajouter un **lien REQ** dans “Dépendances / blocages” dès qu’il y a du flou (cf. `/Docs/requests.md`).