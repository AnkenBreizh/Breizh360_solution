# Tableau de bord — Initiatives (Breizh360)

> **Dernière mise à jour :** 2026-01-09  
> **Règles :** statut standard + **Done = Remise** (voir `Docs/rules.md`)

## Initiatives

| ID | Initiative | Owner | Statut | Dépendances | Prochaine étape | Remise (quand Done) |
|---|---|---|---|---|---|---|
| **INIT-AUTH-001** | Authentification / Autorisation (JWT + RBAC/ABAC) | Responsable Solution | In progress | — | Finaliser les contrats API/Gateway (auth + refresh) + check intégration UI | PR / chemins | 
| **INIT-USR-001** | Users (profil, liste, détail, update) | Responsable Solution | Backlog | INIT-AUTH-001 | Valider contrat Domaine (`USR-REQ-001`) puis contrat API (`USR-REQ-002`) | PR / chemins |
| **INIT-NOTIF-001** | Notifications (SignalR via Gateway) | Responsable Solution | Backlog | INIT-AUTH-001 | Valider contrat Hub/payload (`NOTIF-REQ-001`) puis proxy Gateway (`NOTIF-REQ-002`) | PR / chemins |

## Liens utiles
- Règles : `Docs/rules.md`
- Demandes inter-équipes : `Docs/requests.md`
- Index des interfaces : `Docs/interfaces_index.md`

