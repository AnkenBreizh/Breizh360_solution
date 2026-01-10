using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Breizh360.Api.Metier.Hubs;

/// <summary>
/// Hub SignalR de notifications.
/// Convention : un utilisateur rejoint le groupe <c>user:{userId}</c>.
/// </summary>
[Authorize]
public sealed class NotificationsHub : Hub
{
  public static string UserGroup(Guid userId) => $"user:{userId}";

  public override async Task OnConnectedAsync()
  {
    var raw = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    if (Guid.TryParse(raw, out var userId))
      await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));

    await base.OnConnectedAsync();
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    var raw = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    if (Guid.TryParse(raw, out var userId))
      await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserGroup(userId));

    await base.OnDisconnectedAsync(exception);
  }

  /// <summary>
  /// Ack optionnel - no-op pour l'instant (dépend de la décision inbox persistée ou non).
  /// </summary>
  public Task Ack(string notificationId) => Task.CompletedTask;
}
