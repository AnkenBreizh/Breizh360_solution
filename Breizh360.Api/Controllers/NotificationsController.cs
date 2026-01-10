using System.Security.Claims;
using Breizh360.Api.Metier.Contracts.Notifications;
using Breizh360.Api.Metier.Errors;
using Breizh360.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Breizh360.Api.Metier.Controllers;

/// <summary>
/// Endpoints HTTP autour des notifications.
/// Note : MVP = endpoint de test pour valider le flux SignalR.
/// </summary>
[ApiController]
[Route("notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
  private readonly INotificationsPublisher _publisher;

  public NotificationsController(INotificationsPublisher publisher) => _publisher = publisher;

  /// <summary>
  /// Émet une notification de test vers l'utilisateur courant (SignalR).
  /// </summary>
  [HttpPost("test")]
  [ProducesResponseType(typeof(NotificationsContractsNotificationDto), StatusCodes.Status202Accepted)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> SendTest([FromBody] NotificationsContractsTestNotificationRequest request)
  {
    var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdRaw, out var userId))
    {
      var err = new ErrorsApiError
      {
        Code = "UNAUTHORIZED",
        Message = "Missing or invalid user identifier.",
        Status = StatusCodes.Status401Unauthorized,
        TraceId = HttpContext.TraceIdentifier,
        CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v) ? v.ToString() : null
      };
      return StatusCode(StatusCodes.Status401Unauthorized, err);
    }

    var notification = new NotificationsContractsNotificationDto
    {
      Id = Guid.NewGuid().ToString(),
      Type = request.Type,
      Message = request.Message,
      CreatedAt = DateTimeOffset.UtcNow,
      Metadata = request.Metadata
    };

    await _publisher.PublishToUserAsync(userId, notification, HttpContext.RequestAborted);
    return Accepted(notification);
  }
}
