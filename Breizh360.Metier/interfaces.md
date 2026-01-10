# Interfaces exposées — Métier

> **Dernière mise à jour :** 2026-01-10  
> **Version des contrats :** 0.1.1 (Draft)  
> **Responsable :** Équipe Métier  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + REQ + note de migration

Ce fichier documente les **contrats** consommables par les autres équipes (principalement `Breizh360.Api`).
Un contrat non documenté est considéré comme **inexistant**.

---

## AUTH (Authentification / Autorisation)

### `IF-MET-AUTH-001` — Services Auth (credentials + tokens + autorisation)

- **Statut :** ✅ *Implémenté (v1) — dépendances typées*
- **Responsabilité :**
  - Valider des identifiants (login/email + password)
  - Émettre des tokens (JWT + refresh token) et gérer la rotation
  - Vérifier une permission pour un utilisateur (RBAC/permissions + ABAC minimal)
- **Consommateurs :**
  - `Breizh360.Api` (controllers `AuthController`, filtres d’autorisation)

#### Contrats applicatifs (API publique côté Métier)

- `AuthServiceValidateCredentials`
  - `Task<AuthServiceValidateCredentialsResult<User?>> TryValidateAsync(string loginOrEmail, string password, CancellationToken ct = default)`
  - `Task<User> ValidateOrThrowAsync(string loginOrEmail, string password, CancellationToken ct = default)`

- `TokenService`
  - `Task<TokenPair> IssueAsync(Guid userId, IEnumerable<string>? roles = null, IEnumerable<string>? permissions = null, string? login = null, CancellationToken ct = default)`
  - `Task<TokenPair> RefreshAsync(Guid userId, string refreshToken, IEnumerable<string>? roles = null, IEnumerable<string>? permissions = null, string? login = null, CancellationToken ct = default)`

- `AuthorizationServiceIsAllowed`
  - `Task<bool> IsAllowedAsync(Guid userId, string permission, AuthorizationContext? ctx = null, CancellationToken ct = default)`

> **Note compatibilité** : `TokenService` et `AuthorizationServiceIsAllowed` conservent un constructeur `object` marqué `Obsolete`
> uniquement pour ne pas casser les consommateurs existants. Les appels internes sont 100% typés (aucune reflection).

#### Dépendances attendues (repositories Domaine)

- **Credentials** : `IAuthUserRepository` (namespace `Breizh360.Domaine.Auth.Users`)
  - `GetByLoginAsync(key, ct)`
  - `GetByEmailAsync(key, ct)`

- **Refresh tokens** : `IRefreshTokenRepository` (namespace `Breizh360.Domaine.Auth.RefreshTokens`)
  - `GetByTokenHashAsync(hash, ct)`
  - `AddAsync(token, ct)`
  - `UpdateAsync(token, ct)`

- **Permissions** : `IPermissionRepository` (namespace `Breizh360.Domaine.Auth.Permissions`)
  - `ListForUserAsync(userId, ct)`

#### Règles Métier (résumé)

- **Permissions** :
  - exact match sur le code normalisé
  - wildcard suffixe (ex : `admin.*`)
  - ABAC minimal : suffixe `:own` ⇒ exige `AuthorizationContext.ResourceOwnerUserId == userId`
- **Refresh tokens** :
  - token opaque côté client
  - stockage DB du **hash** uniquement
  - `RefreshAsync` valide + révoque l’ancien token + crée un nouveau token (rotation)

#### Erreurs (minimum)
- `AuthExceptionInvalidCredentials` (identifiants invalides)
- `AuthExceptionUserLocked` (compte verrouillé)
- `SecurityTokenException` (refresh token invalide/expiré)
- `InvalidOperationException` (repository requis manquant)
- `ArgumentException` (entrée invalide)

#### Exemple (pseudo-code)
```csharp
// Login
var result = await authValidate.TryValidateAsync(login, password, ct);
if (!result.IsValid || result.User is null) return Unauthorized();

var pair = await tokenService.IssueAsync(result.User.Id, roles, permissions, login, ct);
return Ok(pair);
```

- **Remise :**
  - `Breizh360.Metier/Auth/AuthServiceValidateCredentials.cs`
  - `Breizh360.Metier/Auth/TokenService.cs`
  - `Breizh360.Metier/Auth/AuthorizationServiceIsAllowed.cs`
  - `Breizh360.Metier/Auth/AuthorizationContext.cs`
  - `Breizh360.Metier/Auth/02_contrat_jwt.md`

---

## USR (Users)

### `IF-MET-USR-001` — Use-cases Users (liste / détail / update)

- **Statut :** ⛔ *Blocked* (contrat listing/pagination à figer)
- **Responsabilité :**
  - Exposer les use-cases Users pour l’API (liste, détail, update)
  - Appliquer les règles métier (validations, invariants, autorisations)
- **Consommateurs :** `Breizh360.Api`
- **Dépendance de clarification :** `Docs/requests.md` → `USR-REQ-003`

- **Contrat (draft, à figer) :**
  - `ListAsync(UserQuery query, CancellationToken ct = default)` : liste paginée
  - `GetAsync(UserId id, CancellationToken ct = default)`
  - `UpdateAsync(UserId id, UserUpdate cmd, CancellationToken ct = default)`

- **Remise :** (à venir) `Breizh360.Metier/Users/...`

---

## NOTIF (Notifications)

### `IF-MET-NOTIF-001` — Use-cases Notifications (inbox persistée)

- **Statut :** 🟡 *Ready côté Domaine/Data* — ⏳ *Backlog Métier*
- **Responsabilité :**
  - Créer/planifier des notifications
  - Gérer l’état (unread/read), retry, expiration, idempotence
- **Consommateurs :** `Breizh360.Api` (hub + endpoints inbox)
- **Dépendances :** `Docs/requests.md` → `NOTIF-REQ-006` (cadrage contrat inbox côté API)

- **Remise :** (à venir) `Breizh360.Metier/Notifications/...`
