using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Breizh360.Api.Metier.Hubs;

[Authorize]
public sealed class NotificationsHub : Hub
{
  public override Task OnConnectedAsync()
  {
    // TODO : rejoindre des groupes selon user/roles si besoin
    return base.OnConnectedAsync();
  }
}
