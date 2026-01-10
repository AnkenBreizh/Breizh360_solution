# Feature NOTIF (Notifications) — bout en bout

> **Date :** 2026-01-09  
> **Initiative :** `INIT-NOTIF-001`  
> **REQ liées :** `NOTIF-REQ-001`, `NOTIF-REQ-002`, `NOTIF-REQ-003`, `NOTIF-REQ-004`, `NOTIF-REQ-005`

## Objectif
Notifications temps réel via SignalR (Hub dans API), proxifiées par la Gateway, consommées par UI.
**Décision : inbox persistée** (historique + unread count + ack/read) — voir `Docs/decisions/ADR-0002-notif-inbox.md`.

## Dépendances
- Auth (JWT) : `INIT-AUTH-001`

## Contrats à publier (minimum)
- **Domaine :** `IF-NOTIF-001` (modèle inbox persistée + invariants + erreurs + repository)
- **Données :** `IF-DATA-NOTIF-001` (tables/index/migrations + implémentation repo EF)
- **Métier :** `IF-MET-NOTIF-001` (use-cases : créer/planifier/ack/read + unread count)
- **API :** `IF-API-NOTIF-001` (Hub + événements + payloads) ; `IF-API-NOTIF-002` (endpoints inbox/unread/mark-as-read)
- **Gateway :** `IF-GATE-NOTIF-001` (proxy `/hubs/*` + WebSockets + corrélation)
- **UI :** `IF-UI-NOTIF-001` (abonnement hub + UX + fallback inbox)

## Suivi / demandes
- Tableau de bord : `Docs/initiatives.md`
- Demandes inter-équipes : `Docs/requests.md`
