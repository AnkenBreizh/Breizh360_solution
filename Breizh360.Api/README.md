# Breizh360.Api — API Métier (Breizh360)

> **Dernière mise à jour :** 2026-01-09  
> **Owner :** Équipe API

## Mission
Expose les **interfaces HTTP** (REST) et **temps réel** (SignalR) du métier Breizh360, en appliquant :
- des **contrats documentés avant implémentation**
- des **IDs stables** (tâches / interfaces)
- la règle **“Fini = Remise”** (une tâche n’est “Done” que si la remise est référencée)

## Périmètre (MVP)
- **USR** : endpoints `/users` (DTO + erreurs)
- **NOTIF** : hub SignalR + événements (notifications temps réel)

> Toute interface non documentée est considérée comme inexistante (voir `05_PROMPT_ApiMetier.txt`).

## Références de suivi
- Suivi des tâches : `./tasks.md`
- Contrats exposés : `./interfaces.md`
- Règles & demandes : `/Docs/rules.md`, `/Docs/requests.md`, `/Docs/interfaces_index.md`, `/Docs/initiatives.md`

## Démarrage local (à compléter)
- Prérequis : .NET SDK, base de données, variables d’environnement
- Lancement : `dotnet run --project Breizh360.Api/Breizh360.Api.csproj`
- Swagger/OpenAPI : `/swagger`

## Conventions de versionning des contrats
- **Patch** : correction non-breaking
- **Minor** : ajout non-breaking
- **Major** : breaking change ⇒ **REQ obligatoire** + **note de migration**

## Checklist “Definition of Done”
- [ ] Contrat `IF-...` complet (routes/méthodes, DTO, erreurs, auth)
- [ ] Tests contractuels / validation DTO
- [ ] Documentation mise à jour (`interfaces.md`, éventuellement `/Docs/feature_*.md`)
- [ ] Remise renseignée (chemin + référence commit/PR)
## Endpoints exposés (actuel)
- Auth : `POST /auth/login`, `POST /auth/refresh`, `POST /auth/logout` (**TODO**: `AUTH-API-001`)
- Profil : `GET /me` (**TODO**: `AUTH-API-002`)
- Notifications (SignalR) : `GET/WS /hubs/notifications` (hub)

## Contrats & suivi
- Contrats : `Breizh360.Api/interfaces.md` (IF-API-…)
- Tâches : `Breizh360.Api/tasks.md`

