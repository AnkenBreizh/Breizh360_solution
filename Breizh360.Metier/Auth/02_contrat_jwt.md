# Contrat JWT + Refresh Tokens (Breizh360)

## Source d’autorité
La validation cryptographique et sémantique du JWT est faite par l’API Métier (Breizh360.Api.Metier).
La Gateway n’est pas source d’autorité. (Elle filtre/forward).

## Configuration attendue
- Jwt:SigningKey
- Jwt:Issuer
- Jwt:Audience

## Durées
- Access token : 15 minutes (par défaut) — configurable
- Refresh token : 14 jours (par défaut) — configurable
- Clock skew : 30 secondes (par défaut) — configurable

## Format des tokens
### Access token (JWT)
- Algorithme : HS256 (HMACSHA256)
- Issuer : Jwt:Issuer
- Audience : Jwt:Audience

#### Claims standards
- sub : UserId (GUID)
- unique_name : login
- jti : identifiant unique du token
- iat : issued-at (epoch seconds)
- exp : expiration

#### Claims applicatifs
- role : rôles utilisateur (ClaimTypes.Role)
- perm : permissions (claim custom répétable)

### Refresh token (opaque)
- Généré aléatoirement (64 bytes min)
- Stocké en base **hashé** (HMACSHA256 avec SigningKey)
- Rotation : à chaque refresh, l’ancien est révoqué et remplacé

## Scénarios d’erreurs attendus
- 401 Unauthorized : access token absent/invalide/expiré
- 401 Unauthorized : refresh token invalide/expiré/révoqué
- 403 Forbidden : JWT valide mais permission manquante

## Endpoints (rappel)
- POST /auth/login -> retourne access + refresh
- POST /auth/refresh -> retourne nouveau access + refresh (rotation)

## Réponses (exemples)
> À compléter côté API (docs/auth/03_exemples_api.md) quand les endpoints seront faits.
