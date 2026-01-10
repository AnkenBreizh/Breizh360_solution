# ADR-0002 — Notifications : inbox persistée

> **Statut :** Accepted  
> **Date :** 2026-01-09  
> **Initiative :** `INIT-NOTIF-001`

## Contexte
Les notifications sont consommées par l’UI en temps réel via SignalR (Hub dans l’API, proxifié par la Gateway).
Il restait à trancher si le système devait être **temps réel only** ou inclure une **inbox persistée**.

## Décision
Nous décidons de **persister** les notifications (inbox) côté back-end.

Cela implique :
- un **modèle domaine** pour une notification persistée (identité, type, contenu, timestamps, état read/unread, idempotence éventuelle)
- une **persistance** (tables + index + migrations)
- des **use-cases** (création, ack/read, unread count, éventuellement purge/expiration)
- des **endpoints** pour l’inbox (lecture/mark-as-read) en plus du **hub** temps réel

## Conséquences
### Avantages
- Historique consultable (centre de notifications / “inbox”)
- Unread count fiable, même après reconnexion / refresh
- Possibilité d’ack/read et de traitement idempotent

### Coûts / risques
- Schéma BDD + migrations à maintenir
- Plus de logique (retry/expiration/purge) à cadrer
- Besoin de contrats API clairs (hub + REST)

## Liens
- Contrat domaine : `Breizh360.Domaine/interfaces.md` → `IF-NOTIF-001`
- Feature : `Docs/feature_notif_b2b.md`
- Demandes : `Docs/requests.md` → `NOTIF-REQ-003`, `NOTIF-REQ-004`, `NOTIF-REQ-005`
