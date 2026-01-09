# Tableau de bord — Initiatives (Breizh360)

> **Dernière mise à jour :** 2026-01-09  
> **Règles :** statut standard + **Done = Remise** (voir `Docs/rules.md`)

## Initiatives

| ID | Initiative | Owner | Statut | Dépendances | Prochaine étape | Remise (quand Done) |
|---|---|---|---|---|---|---|
| **INIT-AUTH-001** | Authentification / Autorisation (JWT + RBAC/ABAC) | Responsable Solution | In progress | `AUTH-REQ-001`, `AUTH-REQ-002` | Finaliser contrats API (`IF-API-AUTH-001`, `IF-API-ME-001`) + routage Gateway si nécessaire | PR / chemins |
| **INIT-USR-001** | Users (profil, liste, détail, update) | Domaine/Métier | Backlog | `USR-REQ-001`, `USR-REQ-002` | Finaliser contrats Domaine (`IF-DOM-USR-001`) puis décliner DTO API/ UI | PR / chemins |
| **INIT-NOTIF-001** | Notifications (SignalR via Gateway) | API/Gateway/UI | In progress | `NOTIF-REQ-001`, `NOTIF-REQ-002`, `ADR-0002` | Trancher inbox (ADR) puis stabiliser events + proxy WebSockets | PR / chemins |


## Liens utiles
- Règles : `Docs/rules.md`
- Demandes inter-équipes : `Docs/requests.md`
- Index des interfaces : `Docs/interfaces_index.md`
