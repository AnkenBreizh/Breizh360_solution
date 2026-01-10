# Tableau de bord — Initiatives (Breizh360)

> **Dernière mise à jour :** 2026-01-09  
> **Règles :** statut standard + **Done = Remise** (voir `Docs/rules.md`)

## Initiatives

| ID | Initiative | Owner | Statut | Dépendances | Prochaine étape | Remise (quand Done) |
|---|---|---|---|---|---|---|
| **INIT-AUTH-001** | Authentification / Autorisation (JWT + RBAC/ABAC) | Responsable Solution | In progress | — | Finaliser les contrats API/Gateway (auth + refresh) + check intégration UI | PR / chemins | 
| **INIT-USR-001** | Users (profil, liste, détail, update) | Responsable Solution | Backlog | INIT-AUTH-001 | Valider contrat Domaine (`USR-REQ-001`) puis contrat API (`USR-REQ-002`) | PR / chemins |
| **INIT-NOTIF-001** | Notifications (SignalR + Inbox persistée) | Responsable Solution | Backlog | `ADR-0002` + `INIT-AUTH-001` + `NOTIF-REQ-001`..`NOTIF-REQ-005` | Publier contrats (Hub/Gateway/Inbox) puis implémenter persistance & endpoints | PR / chemins |

## Liens utiles
- Règles : `Docs/rules.md`
- Demandes inter-équipes : `Docs/requests.md`
- Index des interfaces : `Docs/interfaces_index.md`