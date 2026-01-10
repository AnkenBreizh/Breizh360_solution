# Tableau de bord — Initiatives (Breizh360)

> **Dernière mise à jour :** 2026-01-10  
> **Règles :** statut standard + **Done = Remise** (voir `Docs/rules.md`)

## Initiatives

| ID | Initiative | Owner | Statut | Dépendances | Prochaine étape | Remise (quand Done) |
|---|---|---|---|---|---|---|
| **INIT-AUTH-001** | Authentification / Autorisation (JWT + RBAC/ABAC) | Responsable Solution | In progress | — | Finaliser les contrats API/Gateway (auth + refresh) + check intégration UI | PR / chemins |
| **INIT-USR-001** | Users (profil, liste, détail, update) | Responsable Solution | Backlog | `INIT-AUTH-001` + `USR-REQ-001` + `USR-REQ-002` + `USR-REQ-003` | Figer contrat Domaine de listing (`USR-REQ-003`) puis démarrer use-cases Métier (`USR-MET-001`) | PR / chemins |
| **INIT-NOTIF-001** | Notifications (SignalR + Inbox persistée) | Responsable Solution | Backlog | `ADR-0002` + `INIT-AUTH-001` + `NOTIF-REQ-001`..`NOTIF-REQ-006` | Contrats : OK. Data : persistance inbox livrée (`NOTIF-REQ-004`). Prochaine étape : spec Inbox (`NOTIF-REQ-006`) puis endpoints Inbox + unread count (API) | PR / chemins |

## Liens utiles
- Règles : `Docs/rules.md`
- Demandes inter-équipes : `Docs/requests.md`
- Index des interfaces : `Docs/interfaces_index.md`
