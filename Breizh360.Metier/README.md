# Breizh360.Metier — Couche Métier (use-cases)

> **Dernière mise à jour :** 2026-01-09  
> **Rôle :** Équipe Métier  
> **Règles :** ID stable, statuts standard, **Done = Remise**, contrats avant implémentation.

## Objectif

Ce module porte la **logique applicative** (use-cases) et sert d’interface entre :

- **Domaine** (`Breizh360.Domaine`) : invariants, entités, contrats de repositories
- **Données** (`Breizh360.Data`) : implémentations EF Core / persistance
- **API** (`Breizh360.Api`) : exposition HTTP/SignalR (contrôleurs/hubs)

> Rappel d’architecture : la couche API doit consommer Métier (pas Data) pour conserver le flux
> **Domaine → Données → Métier → API**.

## Périmètre

### AUTH (présent dans ce projet)
Services applicatifs de base pour :
- validation d’identifiants
- émission / rotation de tokens (JWT + refresh token)
- vérification d’autorisations (permissions)

📌 Détails JWT : `Auth/02_contrat_jwt.md`

### USR / NOTIF (à formaliser / implémenter)
- **USR** : use-cases Users (liste / détail / update)
- **NOTIF** : use-cases Notifications (si la persistance “inbox” est confirmée)

## Organisation du code

- `Auth/` : services Auth (login/refresh/autorisation) + contrat JWT
- `Common/` : utilitaires transverses (ex: horloge)
- `interfaces.md` : contrats exposés par Métier (à destination des consommateurs)
- `tasks.md` : suivi (statuts, dépendances, remises)

## Configuration (AUTH)

Clés attendues (voir `Auth/02_contrat_jwt.md`) :
- `Jwt:SigningKey`
- `Jwt:Issuer`
- `Jwt:Audience`

## Suivi

- Contrats : `interfaces.md`
- Backlog / statut : `tasks.md`
