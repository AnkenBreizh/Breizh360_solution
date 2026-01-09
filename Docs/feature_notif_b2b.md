# Feature NOTIF (Notifications) — bout en bout

> **Date :** 2026-01-09  
> **Initiative :** `INIT-NOTIF-001`  
> **REQ liées :** `NOTIF-REQ-001`, `NOTIF-REQ-002`

## Objectif
Notifications temps réel via SignalR (Hub dans API), proxifiées par la Gateway, consommées par UI.
Optionnel : persistance des notifications (inbox) + “unread count”.

## Dépendances
- Auth (JWT) : `INIT-AUTH-001`

## Contrats à publier (minimum)
- API : `IF-API-NOTIF-001` (Hub + événements + payloads) ; `IF-API-NOTIF-002` (endpoints inbox si persistance)
- Gateway : `IF-GATE-NOTIF-001` (proxy `/hubs/*` + WebSockets + corrélation)
- UI : `IF-UI-NOTIF-001` (abonnement hub + UX)
- Domaine/Data/Métier : uniquement si persistance (NOTIF inbox)

## Suivi / demandes
- Tableau de bord : `Docs/initiatives.md`
- Demandes inter-équipes : `Docs/requests.md`
