# Tâches — Passerelle (Breizh360.Gateway)

> **Dernière mise à jour :** 2026-01-10  
> Rappel : **Done = Remise** (chemin vers code/doc/PR). Sans remise → pas fini.

## Backlog / Suivi

| ID | Sujet | Statut | Dépendances / blocages | Remise (quand Done) | DoD (résumé) |
|---|---|---|---|---|---|
| `GATE-DOC-001` | Mettre à niveau les fichiers de suivi (README / tasks / interfaces) | **Done** | — | `Breizh360.Gateway/README.md`, `Breizh360.Gateway/tasks.md`, `Breizh360.Gateway/interfaces.md` | Docs à jour + cohérents (IDs, statuts, remises, contrats) |
| `USR-GATE-001` | Proxy `/api/{**}` (incl. `/api/users`) vers l’API | **Done** | — | `Breizh360.Gateway/Program.cs`, `Breizh360.Gateway/ReverseProxy/GatewayReverseProxyExtensions.cs`, `Breizh360.Gateway/appsettings.json` | Routage OK + propagation `X-Correlation-ID` + smoke tests documentés |
| `NOTIF-GATE-001` | Proxy `/hubs/{**}` + WebSockets/SignalR | **Done** | — | `Breizh360.Gateway/Program.cs`, `Breizh360.Gateway/ReverseProxy/GatewayReverseProxyExtensions.cs`, `Breizh360.Gateway/appsettings.json` | Routage OK + support token navigateur (query) + smoke tests documentés |
| `GATE-SEC-001` | Contrôle léger auth : token présent + format correct sur routes protégées | **Done** | — | `Breizh360.Gateway/Auth/GatewayJwtForwardingMiddleware.cs`, `Breizh360.Gateway/appsettings.json` | Paths anonymes respectés + 401 normalisé + support `access_token` hubs (configurable) |
| `GATE-OBS-001` | Corrélation : émettre/propager `X-Correlation-ID` (scopes logs) | **Done** | — | `Breizh360.Gateway/Observability/GatewayCorrelationIdMiddleware.cs`, `Breizh360.Gateway/Common/CommonCorrelationIdMiddleware.cs`, `Breizh360.Gateway/Program.cs` | Header présent sur toutes les réponses + transmis downstream |
| `GATE-OPS-001` | Rate limiting (par IP) + retour 429 normalisé | **Done** | — | `Breizh360.Gateway/Common/CommonRateLimitingExtensions.cs`, `Breizh360.Gateway/Program.cs`, `Breizh360.Gateway/Errors/GatewayError.cs` | 429 renvoyé + payload JSON `GatewayError` |
| `GATE-OPS-002` | CORS centralisé (origines + credentials) | **Done** | — | `Breizh360.Gateway/Common/CommonCorsExtensions.cs`, `Breizh360.Gateway/Program.cs`, `Breizh360.Gateway/appsettings.json` | Origines contrôlées + fallback dev explicite |
| `GATE-TECH-001` | Clarifier les placeholders (`Common/*`, `ReverseProxy/*`) | **Done** | — | `Breizh360.Gateway/Common/*`, `Breizh360.Gateway/ReverseProxy/GatewayReverseProxyExtensions.cs` | Zéro fichier vide + responsabilité clarifiée |
| `GATE-DEP-001` | Figer les versions NuGet (éviter `Version="*"`) | **Done** | Standardisation solution | `Breizh360.Gateway/Breizh360.Gateway.csproj` | Versions pin + restaurations reproductibles |

## Notes de pilotage
- Associer chaque item “exposé” à un contrat `IF-GATE-...` dans `interfaces.md`.
- Dès qu’un point est flou côté inter-équipes : créer une demande dans `Docs/requests.md` (règle “pas de suppositions”).
