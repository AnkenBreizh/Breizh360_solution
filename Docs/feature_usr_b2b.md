# Feature USR (Users) — bout en bout

> **Date :** 2026-01-09  
> **Initiative :** `INIT-USR-001`  
> **REQ liées :** `USR-REQ-001`, `USR-REQ-002`

## Objectif
Gestion des utilisateurs (profil, liste, détail, mise à jour) via API + Gateway + UI, avec permissions.

## Dépendances
- Auth (JWT + identité interne) : `INIT-AUTH-001`

## Endpoints (cibles)
- `GET /users` (liste, paginée)
- `GET /users/{id}`
- `PUT /users/{id}` (mise à jour profil)
- `GET /me` (déjà côté Auth, réutilisé pour profil courant)

## Contrats à publier (minimum)
- Domaine : `IF-USR-001` (entités + invariants) ; `IF-USR-002` (repos)
- Data : `IF-DATA-USR-001` (migrations/config) ; `IF-DATA-USR-002` (repos concrets)
- Métier : `IF-MET-USR-001` (use cases Users)
- API : `IF-API-USR-001` (routes + DTO + erreurs)
- Gateway : `IF-GATE-USR-001` (proxy `/api/users` + corrélation)
- UI : `IF-UI-USR-001` (écrans + états + erreurs)

## Suivi / demandes
- Tableau de bord : `Docs/initiatives.md`
- Demandes inter-équipes : `Docs/requests.md`
