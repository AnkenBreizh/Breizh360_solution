# Tâches — Passerelle (Breizh360.Gateway)

> **Dernière mise à jour :** 2026-01-09  
> Rappel : **Done = Remise** (chemin vers code/doc/PR). Sans remise → pas fini.

## Backlog / Suivi

| ID | Sujet | Statut | Dépendances / blocages | Remise (quand Done) | DoD (résumé) |
|---|---|---|---|---|---|
| `GATE-DOC-001` | Mettre à niveau les fichiers de suivi (README / tasks / interfaces) | **In review** | — | `Breizh360.Gateway/README.md`, `Breizh360.Gateway/tasks.md`, `Breizh360.Gateway/interfaces.md` | Docs à jour + cohérents (IDs, statuts, remises, contrats) |
| `USR-GATE-001` | Proxy `/api/{**}` (incl. `/api/users`) vers l’API | **In review** | Contrats API Users stabilisés (côté API) | `Breizh360.Gateway/Program.cs`, `Breizh360.Gateway/appsettings.json` | Routage OK + propagation `X-Correlation-ID` + smoke test (curl/UI) |
| `NOTIF-GATE-001` | Proxy `/hubs/{**}` + WebSockets/SignalR | **In review** | Contrats Hub/payload stabilisés (côté API) | `Breizh360.Gateway/Program.cs`, `Breizh360.Gateway/appsettings.json` | Connexion hub via Gateway (smoke) + corrélation propagée |
| `GATE-SEC-001` | Contrôle léger auth : `Authorization: Bearer <token>` sur routes protégées | **In review** | — | `Breizh360.Gateway/Auth/GatewayJwtForwardingMiddleware.cs`, `Breizh360.Gateway/appsettings.json` | Paths anonymes respectés + erreurs 401 normalisées |
| `GATE-OBS-001` | Corrélation : émettre/propager `X-Correlation-ID` (scopes logs) | **In review** | — | `Breizh360.Gateway/Observability/GatewayCorrelationIdMiddleware.cs`, `Breizh360.Gateway/Program.cs` | Header présent sur toutes les réponses + transmis downstream |
| `GATE-OPS-001` | Rate limiting (par IP) + retour 429 normalisé | **In review** | — | `Breizh360.Gateway/Program.cs`, `Breizh360.Gateway/Errors/GatewayErrorMiddleware.cs` | 429 renvoyé + payload JSON GatewayError |
| `GATE-OPS-002` | CORS centralisé (origines + credentials) | **In review** | — | `Breizh360.Gateway/Program.cs`, `Breizh360.Gateway/appsettings.json` | Origines contrôlées + comportement dev explicite |
| `GATE-TECH-001` | Clarifier les placeholders `Common/*` (supprimer ou implémenter) | **Backlog** | — | `Breizh360.Gateway/Common/*` | Zéro fichier vide en prod (ou impl. réelle) |
| `GATE-DEP-001` | Figer les versions NuGet (éviter `Version="*"`) | **Backlog** | Standardisation solution | `Breizh360.Gateway/Breizh360.Gateway.csproj` | Versions pin + restaurations reproductibles |

## Notes de pilotage
- Associer chaque item “exposé” à un contrat `IF-GATE-...` dans `interfaces.md`.
- Dès qu’un point est flou côté inter-équipes : créer une demande dans `Docs/requests.md` (règle “pas de suppositions”).
