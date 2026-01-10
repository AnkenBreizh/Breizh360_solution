# Interfaces exposées — Métier

> **Dernière mise à jour :** 2026-01-09  
> **Version des contrats :** 0.1.0 (Draft)  
> **Responsable :** Équipe Métier  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + REQ + note de migration

Ce fichier documente les **contrats** consommables par les autres équipes (principalement `Breizh360.Api`).
Un contrat non documenté est considéré comme **inexistant**.

---

## AUTH (Authentification / Autorisation)

### `IF-MET-AUTH-001` — Services Auth (credentials + tokens + autorisation)

- **Statut :** 🚧 *Implémenté (première version) / à stabiliser (contrats repos)*
- **Responsabilité :**
  - Valider des identifiants (login/email + password)
  - Émettre des tokens (JWT + refresh token) et gérer la rotation
  - Vérifier une permission pour un utilisateur (RBAC/permissions)
- **Consommateurs :**
  - `Breizh360.Api` (controllers `AuthController`, filtres d’autorisations)
  - Tests/outils (à venir)

#### Contrat applicatif (API publique côté Métier)

- `AuthServiceValidateCredentials`
  - `Task<AuthServiceValidateCredentialsResult<dynamic>> TryValidateAsync(string loginOrEmail, string password, CancellationToken ct = default)`
  - `Task<dynamic> ValidateOrThrowAsync(string loginOrEmail, string password, CancellationToken ct = default)`
- `TokenService`
  - `Task<TokenPair> IssueAsync(Guid userId, IEnumerable<string>? roles = null, IEnumerable<string>? permissions = null, string? login = null, CancellationToken ct = default)`
  - `Task<TokenPair> RefreshAsync(Guid userId, string refreshToken, IEnumerable<string>? roles = null, IEnumerable<string>? permissions = null, string? login = null, CancellationToken ct = default)`
- `AuthorizationServiceIsAllowed`
  - `Task<bool> IsAllowedAsync(Guid userId, string permission, AuthorizationContext? ctx = null, CancellationToken ct = default)`

> **Note importante (à stabiliser)** : certaines dépendances repos sont actuellement injectées sous forme de `object`
> et consommées en “best effort” (reflection) pour éviter un conflit de nom (`IUserRepository` AUTH vs USR).
> Le contrat attendu doit être figé côté Domaine afin de supprimer `dynamic`/reflection.

#### Dépendances attendues (repositories)

Tant que la stabilisation n’est pas faite, les repositories doivent exposer **au moins une** des signatures/variantes suivantes :

- **User lookup (credentials)** : `GetByLoginOrEmailAsync` / `FindByLoginOrEmailAsync` / `GetByLoginAsync` / `GetByEmailAsync` (ou équivalents)
- **Refresh tokens**
  - Stockage : `CreateAsync` / `AddAsync` / `InsertAsync` / `StoreAsync` / `UpsertAsync`
  - Validation/consommation (retour `bool`) : `ConsumeAsync` / `ValidateAsync` / `IsValidAsync` / `ExistsAsync` / `CheckAsync` / `CanRefreshAsync`
- **Permissions**
  - Direct : `IsAllowedAsync` / `HasPermissionAsync` / `CheckAsync` (retour `bool?`)
  - Ou liste : `GetPermissionsForUserAsync` / `ListPermissionsAsync` / `GetEffectivePermissionsAsync`

#### Erreurs (minimum)
- `AuthExceptionInvalidCredentials` (identifiants invalides)
- `AuthExceptionUserLocked` (compte verrouillé)
- `SecurityTokenException` (refresh token invalide/expiré)
- `InvalidOperationException` (contrats repos incomplets / méthode non trouvée)
- `ArgumentException` (entrée invalide)

#### Exemple (pseudo-code)
```csharp
// Login
var result = await authValidate.TryValidateAsync(login, password, ct);
if (!result.Success) return Unauthorized();

var pair = await tokenService.IssueAsync(result.UserId, roles, permissions, login, ct);
return Ok(pair);
```

- **Remise :**
  - `Breizh360.Metier/Auth/AuthServiceValidateCredentials.cs`
  - `Breizh360.Metier/Auth/TokenService.cs`
  - `Breizh360.Metier/Auth/AuthorizationServiceIsAllowed.cs`
  - `Breizh360.Metier/Auth/02_contrat_jwt.md`

---

## USR (Users)

### `IF-MET-USR-001` — Use-cases Users (liste / détail / update)

- **Statut :** ⏳ *Backlog / Blocked (contrats listing/pagination à clarifier)*
- **Responsabilité :**
  - Exposer les use-cases Users pour l’API (liste, détail, update)
  - Appliquer les règles métier (validations, invariants, autorisations)
- **Consommateurs :** `Breizh360.Api`
- **Contrat (draft, à figer) :**
  - `ListAsync(...)` : liste paginée (contrat pagination + tri + filtres à définir)
  - `GetAsync(UserId id, ...)`
  - `UpdateAsync(UserId id, ...)` (maj profil)

- **Erreurs :** (à compléter) `NotFound`, validation, forbidden
- **Remise :** (à venir) `Breizh360.Metier/Users/...`

---

## NOTIF (Notifications)

### `IF-MET-NOTIF-001` — Use-cases Notifications (inbox persistée)

- **Statut :** 🟡 *Ready* (inbox persistée acceptée via ADR-0002)
- **Responsabilité :**
  - Créer/planifier des notifications (si inbox persistée)
  - Gérer l’état (unread/read), retry, expiration, idempotence
- **Consommateurs :** `Breizh360.Api` (hub + endpoints inbox si option activée)
- **Contrat :** conforme au domaine `IF-NOTIF-001` + persistence `IF-DATA-NOTIF-001`
- **Erreurs :** (à compléter)
- **Remise :** (à venir) `Breizh360.Metier/Notifications/...`


**API proposée (exemple)**
```csharp
namespace Breizh360.Metier.Notifications;

public interface INotificationsService
{
    Task CreateAsync(NotificationToCreate cmd, CancellationToken ct = default);
    Task<IReadOnlyList<NotificationDto>> ListAsync(NotificationQuery query, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(UserId userId, CancellationToken ct = default);
    Task MarkAsReadAsync(NotificationId id, CancellationToken ct = default);
}
```
