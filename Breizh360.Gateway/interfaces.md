# Interfaces exposées — Passerelle (Breizh360.Gateway)

> **Dernière mise à jour :** 2026-01-10  
> **Version des contrats :** 0.3.0 (Draft)  
> **Responsable :** Équipe Passerelle  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + demande (REQ) + note de migration

## Convention
- IDs de contrats : `IF-GATE-<DOMAINE>-###` (ex: `IF-GATE-USR-001`).
- La Passerelle **n’altère pas** les payloads métier : les contrats fonctionnels (DTO, erreurs métier) sont la responsabilité de l’API.
- La Passerelle garantit des **comportements transverses** (routing, headers, erreurs Gateway).

---

## META

### `IF-GATE-HEALTH-001` — Healthcheck Passerelle
- **Responsabilité :** endpoint de santé local (sans proxy).
- **Consommateurs :** infra / monitoring / dev.
- **Route :** `GET /health`
- **Réponse (200) :**
  - JSON `{ "status": "ok", "correlationId": "..." }`
  - Header `X-Correlation-ID` toujours présent.
- **Erreurs :** aucune (hors indisponibilité de l’hôte).
- **Remise :** `Breizh360.Gateway/Program.cs`

---

## USR (Users)

### `IF-GATE-USR-001` — Proxy HTTP API (`/api/{**}`)
- **Responsabilité :** forward des requêtes HTTP(s) vers le cluster API.
- **Consommateurs :** UI / clients.
- **Contrat :**
  - **Entrée :** `/api/{**catch-all}`
  - **Sortie :** même path et query string, vers la destination configurée (`ReverseProxy:Clusters:api`).
  - **Méthodes :** toutes (GET/POST/PUT/PATCH/DELETE…).
  - **Transformations :** propagation de `X-Correlation-ID` (voir `IF-GATE-OBS-001`).
- **Auth (côté Gateway) :**
  - Si route protégée : présence + format `Authorization: Bearer <jwt>` requis (**contrôle léger**).
  - Exemptions via `Gateway:Auth:AnonymousPathPrefixes`.
- **Erreurs générées par la Passerelle :**
  - `401` si token manquant/invalid (payload `GatewayError`).
  - `429` si rate limit atteint (payload `GatewayError`).
  - `502` si exception non gérée dans la pipeline Gateway (payload `GatewayError`).
- **Références (métier) :** contrats API Users (DTO/erreurs) côté **API**.
- **Remise :**
  - `Breizh360.Gateway/Program.cs`
  - `Breizh360.Gateway/appsettings.json`

---

## NOTIF (Notifications)

### `IF-GATE-NOTIF-001` — Proxy SignalR (`/hubs/{**}`)
- **Responsabilité :** forward des connexions SignalR (WebSockets/LongPolling) vers le cluster API.
- **Consommateurs :** UI / clients.
- **Contrat :**
  - **Entrée :** `/hubs/{**catch-all}`
  - **Sortie :** même path et query string vers `ReverseProxy:Clusters:api`.
  - **Transport :** WebSockets supporté par YARP, fallback HTTP si nécessaire (selon client SignalR).
  - **Transformations :** propagation `X-Correlation-ID`.
- **Auth (côté Gateway) :**
  - Contrôle léger du token :
    - `Authorization: Bearer <jwt>` (**recommandé**)
    - **ou** `access_token=<jwt>` en querystring (SignalR navigateur), si `Gateway:Auth:AllowAccessTokenQueryForHubs=true`.
  - Exemptions via `Gateway:Auth:AnonymousPathPrefixes`.
  - ⚠️ L’API reste responsable de l’extraction/validation du token pour les hubs (ex: `JwtBearerEvents.OnMessageReceived`).
- **Références (métier) :** contrats Hub/payload côté **API**.
- **Remise :**
  - `Breizh360.Gateway/Program.cs`
  - `Breizh360.Gateway/appsettings.json`
  - `Breizh360.Gateway/Auth/GatewayJwtForwardingMiddleware.cs`

---

## Observabilité / Erreurs (transverse)

### `IF-GATE-OBS-001` — Corrélation `X-Correlation-ID`
- **Responsabilité :** assurer une corrélation stable bout-en-bout.
- **Règles :**
  - Si le client fournit `X-Correlation-ID`, il est conservé.
  - Sinon, la Passerelle en génère un et l’injecte.
  - Le header est renvoyé sur **toutes** les réponses Gateway.
  - Le header est propagé vers l’API via transform YARP.
- **Remise :**
  - `Breizh360.Gateway/Observability/GatewayCorrelationIdMiddleware.cs`
  - `Breizh360.Gateway/ReverseProxy/GatewayReverseProxyExtensions.cs`

### `IF-GATE-ERR-001` — Erreurs Gateway normalisées
- **Responsabilité :** normaliser les erreurs générées par la Passerelle.
- **Format :**
  - `application/json`
  - Payload : `GatewayError { code, message, correlationId }`
- **Codes (actuels) :**
  - `AUTH_MISSING` / `AUTH_INVALID` (401)
  - `RATE_LIMITED` (429)
  - `GW_UNHANDLED` (502)
- **Remise :**
  - `Breizh360.Gateway/Errors/GatewayError.cs`
  - `Breizh360.Gateway/Errors/GatewayErrorMiddleware.cs`
  - `Breizh360.Gateway/Auth/GatewayJwtForwardingMiddleware.cs`
  - `Breizh360.Gateway/Common/CommonRateLimitingExtensions.cs`
