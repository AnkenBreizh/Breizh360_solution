# Passerelle — Breizh360 (Breizh360.Gateway)

> **Dernière mise à jour :** 2026-01-09  
> **Rôle :** point d’entrée HTTP(S) pour l’UI / clients, basé sur **YARP** (reverse proxy).

## Objectif
La Passerelle (Gateway) sert de **façade unique** et a vocation à :
- **Proxy** vers l’API :
  - `/api/{**catch-all}` → cluster **API**
  - `/hubs/{**catch-all}` → cluster **API** (SignalR / WebSockets)
- Appliquer des **cross-cutting concerns** centralisés :
  - Corrélation : `X-Correlation-ID` (émission + propagation vers l’API)
  - CORS centralisé (origines autorisées via configuration)
  - Rate limiting (par IP)
  - Contrôle léger d’auth : présence/format du header `Authorization: Bearer <token>` sur les routes protégées
  - Normalisation des erreurs **générées par la Passerelle**

> ⚠️ La Passerelle **n’est pas** la source d’autorité pour la validation JWT : la validation cryptographique/claims reste côté **API**.

## Démarrage rapide (local)
1. Démarrer l’API (destination par défaut) sur `https://localhost:5101`.
2. Démarrer la Passerelle (ports par défaut) :
   - HTTPS : `https://localhost:5001`
   - HTTP  : `http://localhost:5000`

### Endpoints utiles
- `GET /health` → statut + `correlationId` (non rate-limité).

## Configuration (appsettings.json)
- `Gateway:Cors:AllowedOrigins` : liste d’origines autorisées (CORS + credentials).
- `Gateway:Auth:AnonymousPathPrefixes` : préfixes exemptés de contrôle `Authorization` (ex: `/api/auth`, `/health`).
- `Gateway:RateLimiting:*` : paramètres du rate limiting.
- `ReverseProxy:*` : routes/clusters YARP (destination API, règles de match).

## Évolution / Ajout de routes
1. Ajouter/adapter une route YARP dans `ReverseProxy:Routes`.
2. Documenter le contrat dans `interfaces.md` (IDs `IF-GATE-...`).
3. Mettre à jour le suivi `tasks.md` (ID stable + **Done = Remise**).

## Fichiers de suivi
- `tasks.md` : backlog, statuts, remises.
- `interfaces.md` : contrats exposés par la Passerelle (routing + headers + erreurs Gateway).
