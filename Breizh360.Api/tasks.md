# Tâches — API Métier

> **Dernière mise à jour :** 2026-01-09  
> Rappel : **Fini = Remise**. Ne pas marquer “Done” sans lien vers une remise vérifiable.

## Backlog / Suivi

| ID | Sujet | Statut | Dépendances / blocages | Livrable (remise attendue) | DoD (résumé) |
|---|---|---|---|---|---|
| `USR-API-001` | Endpoints `/users` (DTO + erreurs) | Backlog | Contrat `IF-API-USR-001` à finaliser | `Breizh360.Api/Users/...` | Contrat + DTO + erreurs + doc + remise |
| `NOTIF-API-001` | Hub SignalR + événements | Backlog | Décision d’architecture NOTIF (inbox persistée ? ack/retry ?) + contrat `IF-API-NOTIF-001` | `Breizh360.Api/Notifications/...` (ou `Hubs/...`) | Contrat + méthodes hub + payload + doc + remise |

## Notes de pilotage
- Associer chaque tâche à **un contrat `IF-...`** (ou plusieurs si besoin), et inversement.
- Ajouter un **lien REQ** dans “Dépendances / blocages” dès qu’il y a du flou (cf. `/Docs/requests.md`).
