using System;
using System.Threading;
using System.Threading.Tasks;
using Breizh360.Api.Metier.Contracts.Notifications;
using Breizh360.Api.Metier.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Breizh360.Api.Services;

/// <summary>
/// Service de publication de notifications (SignalR).
/// </summary>
public interface INotificationsPublisher
{
  Task PublishToUserAsync(Guid userId, NotificationsContractsNotificationDto notification, CancellationToken ct = default);
}

/// <summary>
/// Implémentation basée sur SignalR.
/// Convention : un utilisateur est inscrit dans le groupe <c>user:{userId}</c>.
/// </summary>
public sealed class NotificationsPublisher : INotificationsPublisher
{
  private readonly IHubContext<NotificationsHub> _hub;

  public NotificationsPublisher(IHubContext<NotificationsHub> hub) => _hub = hub;

  public Task PublishToUserAsync(Guid userId, NotificationsContractsNotificationDto notification, CancellationToken ct = default)
    => _hub.Clients.Group(NotificationsHub.UserGroup(userId))
      .SendAsync("notification.received", notification, ct);
}
