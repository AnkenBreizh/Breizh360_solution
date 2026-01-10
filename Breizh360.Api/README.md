# Breizh360.Api — API Métier (Breizh360)

> **Dernière mise à jour :** 2026-01-10  
> **Owner :** Équipe API

## Mission
Expose les **interfaces HTTP** (REST) et **temps réel** (SignalR) du métier Breizh360, en appliquant :
- des **contrats documentés avant implémentation**
- des **IDs stables** (tâches / interfaces)
- la règle **“Fini = Remise”**

## Périmètre implémenté (MVP)
### USR
- `GET /users` (pagination + filtre `q`)
- `GET /users/{id}`

### NOTIF
- Hub SignalR : `/hubs/notifications`
- Événement : `notification.received`
- Endpoint de test : `POST /notifications/test`

## Fichiers de suivi
- Suivi des tâches : `./tasks.md`
- Contrats exposés : `./interfaces.md`
- Règles & demandes : `../Docs/rules.md`, `../Docs/requests.md`

## Démarrage local (indications)
- Lancement : `dotnet run --project Breizh360.Api/Breizh360.Api.csproj`
- Swagger (DEV) : `/swagger`
- Healthcheck : `/health`

## Notes SignalR (clients)
- Auth via header `Authorization: Bearer <token>` ou query `?access_token=<token>` (pour hubs).
- Les notifications sont poussées au groupe `user:{userId}` et exposées via `notification.received`.
