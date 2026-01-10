# Interfaces exposées — Domaine

> **Dernière mise à jour :** 2026-01-10  
> **Version des contrats :** 0.2.0 (Draft)  
> **Responsable :** Équipe Domaine  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + REQ + note de migration

## AUTH (Authentification / Autorisation)

### `IF-AUTH-001` — Contrats domaine Auth (Users, Roles, Permissions, RefreshTokens)

- **Responsabilité :**
  - Modéliser l’authentification et l’autorisation (invariants, entités, value objects)
  - Exposer des **contrats de persistance** dédiés (repositories) par agrégat
- **Contrats exposés :**
  - `IAuthUserRepository` *(AUTH)* — accès au user d’authentification (login/email, soft delete, rôles)
  - `IRoleRepository`, `IPermissionRepository`, `IRefreshTokenRepository`
- **Note compatibilité :**
  - `IUserRepository` dans `Breizh360.Domaine.Auth.Users` est conservé comme **alias historique** marqué `[Obsolete]`.
  - Le domaine USR conserve `Breizh360.Domaine.Users.Repositories.IUserRepository` pour le profil métier.

## USR (Users)

### `IF-USR-001` — Contrat domaine Users (Aggregate + Repository)

- **Responsabilité :**
  - Définir le **modèle de domaine Users** (invariants, value objects)
  - Exposer le **contrat de persistance** via `IUserRepository` (côté domaine)

- **Consommateurs :**
  - `Breizh360.Data` (implémentation EF / SQL du repository)
  - `Breizh360.Metier` (cas d’usage / services métier)

- **Contrat :**
  - Types : `User`, `UserId`, `Email`, `DisplayName`
  - Repository : `IUserRepository`

```csharp
namespace Breizh360.Domaine.Users.Repositories;

public interface IUserRepository
{
    Task<Entities.User?> GetByIdAsync(ValueObjects.UserId id, CancellationToken ct = default);
    Task<Entities.User?> GetByEmailAsync(ValueObjects.Email email, CancellationToken ct = default);

    Task AddAsync(Entities.User user, CancellationToken ct = default);
    Task UpdateAsync(Entities.User user, CancellationToken ct = default);
    Task DeleteAsync(ValueObjects.UserId id, CancellationToken ct = default);
}
```

- **Exemple :**
```csharp
using Breizh360.Domaine.Users.Entities;
using Breizh360.Domaine.Users.ValueObjects;
using Breizh360.Domaine.Users.Repositories;

var user = User.Create(
    id: UserId.New(),
    email: Email.From("alice@example.com"),
    displayName: DisplayName.From("Alice")
);

await userRepository.AddAsync(user, ct);
```

- **Erreurs :**
  - Validation d’invariants : `DomainException` (ou `ArgumentException` si utilisé directement)
  - Unicité email : à faire respecter par l’implémentation Data (unique index), et remontée via couche Métier/API

- **Remise :**
  - `Breizh360.Domaine/Users/Entities/User.cs`
  - `Breizh360.Domaine/Users/ValueObjects/UserId.cs`
  - `Breizh360.Domaine/Users/ValueObjects/Email.cs`
  - `Breizh360.Domaine/Users/ValueObjects/DisplayName.cs`
  - `Breizh360.Domaine/Users/Repositories/IUserRepository.cs`

## NOTIF (Notifications)

### `IF-NOTIF-001` — Contrat domaine Notifications (Inbox persistée)

- **Statut :** ✅ *Décision validée (Inbox persistée)* / 🚧 *Implémentation à faire*
- **Décision :** `Docs/decisions/ADR-0002-notif-inbox.md`
- **Responsabilité :**
  - Modéliser une **inbox de notifications persistée** (cycle de vie, retry, expiration)
  - Garantir la **traçabilité** (audit) et la **rejouabilité**
  - Encadrer l’**idempotence** (anti-doublons) via une clé fonctionnelle si nécessaire
- **Consommateurs :**
  - `Breizh360.Data` (persistance EF/SQL, index/contraintes)
  - `Breizh360.Metier` (use-cases : créer, planifier, envoyer, relancer, marquer)
  - `Breizh360.Api` (exposition éventuelle via Métier : inbox utilisateur, actions)
- **Contrat :**
  - Types : `Notifications.Entities.Notification`, `Notifications.ValueObjects.NotificationId`,
    `Notifications.ValueObjects.NotificationType`, `Notifications.ValueObjects.NotificationStatus`
  - Repository : `INotificationRepository`
  - Sender/Dispatcher : `INotificationSender`

```csharp
namespace Breizh360.Domaine.Notifications.Repositories;

public interface INotificationRepository
{
    Task<Entities.Notification?> GetByIdAsync(ValueObjects.NotificationId id, CancellationToken ct = default);

    /// <summary>
    /// Récupère un lot de notifications à traiter (Pending et échéance atteinte).
    /// Le paramètre utcNow est passé par la couche applicative (testabilité).
    /// </summary>
    Task<IReadOnlyList<Entities.Notification>> FindPendingDueAsync(
        DateTime utcNow,
        int limit,
        CancellationToken ct = default);

    /// <summary>
    /// Anti-doublon (Optionnel mais recommandé si plusieurs sources peuvent produire la même notif).
    /// À faire respecter par l'implémentation Data (index unique ou contrainte).
    /// </summary>
    Task<bool> ExistsByIdempotencyKeyAsync(
        Guid userId,
        string idempotencyKey,
        CancellationToken ct = default);

    Task AddAsync(Entities.Notification notification, CancellationToken ct = default);
    Task UpdateAsync(Entities.Notification notification, CancellationToken ct = default);
}
```

```csharp
namespace Breizh360.Domaine.Notifications.Senders;

public interface INotificationSender
{
    /// <summary>
    /// Envoi effectif sur le(s) canal(aux) cible(s).
    /// La persistance et les transitions de statut sont gérées par la couche applicative + domaine.
    /// </summary>
    Task SendAsync(Notifications.Entities.Notification notification, CancellationToken ct = default);
}
```

- **Erreurs / règles :**
  - Invariants de construction et transitions de statut : `DomainException`
  - Unicité `IdempotencyKey` (si activée) : imposée par Data (index unique), remontée via Métier/API
  - Workflow recommandé : `Pending → Sent` | `Pending → Failed (avec retry)` | `Pending → Expired/Cancelled`
- **Remise (à produire) :**
  - `Breizh360.Domaine/Notifications/Entities/Notification.cs`
  - `Breizh360.Domaine/Notifications/ValueObjects/NotificationId.cs`
  - `Breizh360.Domaine/Notifications/ValueObjects/NotificationType.cs`
  - `Breizh360.Domaine/Notifications/ValueObjects/NotificationStatus.cs`
  - `Breizh360.Domaine/Notifications/Repositories/INotificationRepository.cs`
  - `Breizh360.Domaine/Notifications/Senders/INotificationSender.cs`
  - (si retenu) `Breizh360.Domaine/Notifications/ValueObjects/IdempotencyKey.cs`
