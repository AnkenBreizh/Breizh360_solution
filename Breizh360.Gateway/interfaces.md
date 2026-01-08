# Interfaces exposées — Passerelle

> **Dernière mise à jour : 08 / 01 / 2026**


## IF-GW-001 — Reverse proxy (YARP) : /api/* et /hubs/*
- **Consommateurs :** UI, Tests
- **Contrat / exemples :**

Contrat : la Gateway proxifie /api/{**} et /hubs/{**} vers l’API ( https://localhost:5101 en dev). WebSockets supportés pour SignalR. Headers : propage Authorization et X-Correlation-ID vers l’API. Exemples : API via Gateway : https://localhost:5001/api/... ; Hub via Gateway : wss://localhost:5001/hubs/...

- **Remise :** Breizh360.Gateway/Program.cs + Breizh360.Gateway/appsettings.json + docs/gateway/reverse_proxy.md

## IF-GW-002 — CORS + Rate limiting (technique)
- **Consommateurs :** UI, API
- **Contrat / exemples :**

CORS : politique unique côté Gateway (origins configurables) appliquée à /api et /hubs . Rate limit : stratégie par défaut (HTTP 429) ; exceptions possibles (ex : health, login) via config. Erreurs : 429 renvoie un corps JSON GatewayError avec correlationId .

- **Remise :** Breizh360.Gateway/Program.cs + docs/gateway/interfaces.md

## IF-GW-003 — Corrélation : X-Correlation-ID
- **Consommateurs :** Tous
- **Contrat / exemples :**

Contrat : si le client fournit X-Correlation-ID , il est conservé ; sinon la Gateway en génère un (GUID) et le renvoie dans la réponse. Logs : le correlation id est mis en scope de logs et propagé aux appels proxifiés.

- **Remise :** Breizh360.Gateway/Observability/GatewayCorrelationIdMiddleware.cs + docs/gateway/interfaces.md
