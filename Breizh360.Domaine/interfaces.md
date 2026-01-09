# Interfaces exposées — Domaine

> 2026-01-09

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

### `IF-NOTIF-001` — Contrat domaine Notifications (Inbox)

- **Statut :** ⏳ *Backlog (optionnel)*
- **Responsabilité :** Modéliser une inbox persistée (si décision validée).
- **Consommateurs :** `Breizh360.Data`, `Breizh360.Metier`
- **Contrat / Exemple / Erreurs / Remise :** à compléter une fois la décision prise.
