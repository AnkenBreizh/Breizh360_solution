# Interfaces exposées — Interface utilisateur (Breizh360.UI)

> **Dernière mise à jour :** 2026-01-09  
> **Version des contrats :** 0.1.0 (Draft)  
> **Responsable :** Équipe UI  
> **Règle de changement :** breaking change ⇒ nouvelle version majeure + REQ + note de migration

> Ce fichier documente les **contrats internes UI** (clients typés/services) qui encapsulent la consommation API/Gateway.  
> Objectif : permettre aux pages/composants de rester stables même si les détails réseau évoluent.

## USR (Users)

### `IF-USR-001` — Client UI Users (consommation API)
- **Statut :** Draft (bloqué tant que `USR-REQ-002` n’est pas Done)
- **Responsabilité :** fournir les données Users nécessaires aux écrans *liste* et *détail*.
- **Consommateurs :** pages/components Users (`USR-UI-001`).
- **Contrat (proposé) :**
```csharp
namespace Breizh360.UI.Users.Clients;

public interface IUsersClient
{
    Task<PagedResult<UserSummaryVm>> GetUsersAsync(UsersQuery query, CancellationToken ct = default);
    Task<UserDetailsVm> GetUserAsync(Guid id, CancellationToken ct = default);
}

public sealed record UsersQuery(int Page = 1, int PageSize = 20, string? Search = null);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);

public sealed record UserSummaryVm(Guid Id, string DisplayName);
public sealed record UserDetailsVm(Guid Id, string DisplayName, string Email);
```
- **Erreurs / comportement :**
  - `401/403` ⇒ déclencher un flux de ré-authentification (ou écran « accès refusé » selon décision AUTH).
  - `404` (détail) ⇒ afficher un état « user introuvable ».
  - `500` ⇒ message générique + log.
- **Remise attendue :**
  - `Breizh360.UI/Users/Clients/IUsersClient.cs`
  - `Breizh360.UI/Users/Models/*`
  - `Breizh360.UI/Users/Clients/HttpUsersClient.cs` (ou équivalent)

## NOTIF (Notifications)

### `IF-UI-NOTIF-001` — Client UI Notifications (SignalR via Gateway)
- **Statut :** Draft (bloqué tant que `NOTIF-REQ-001` + `NOTIF-REQ-002` ne sont pas Done)
- **Responsabilité :** gérer la connexion au hub + exposer un flux d’événements consumable par l’UI.
- **Consommateurs :** service d’affichage/toasts + center (`NOTIF-UI-001`).
- **Contrat (proposé) :**
```csharp
namespace Breizh360.UI.Notifications.Clients;

public interface INotificationsHubClient : IAsyncDisposable
{
    Task ConnectAsync(CancellationToken ct = default);
    Task DisconnectAsync(CancellationToken ct = default);

    event Action<NotificationVm> NotificationReceived;

    // Optionnel si inbox persistée (décision) :
    // Task AckAsync(Guid notificationId, CancellationToken ct = default);
}

public sealed record NotificationVm(Guid Id, string Type, string Message, DateTimeOffset CreatedAt);
```
- **Erreurs / comportement :**
  - Reconnexion : retry simple + backoff (à préciser quand contrat hub stabilisé).
  - Auth : token JWT à fournir lors de la connexion (selon IF-API-NOTIF-… et AUTH).
  - Payload invalide ⇒ ignorer + log.
- **Remise attendue :**
  - `Breizh360.UI/Notifications/Clients/INotificationsHubClient.cs`
  - `Breizh360.UI/Notifications/Models/*`
  - `Breizh360.UI/Notifications/Clients/SignalRNotificationsHubClient.cs` (ou équivalent)
