using System;
using System.Collections.Generic;

namespace Breizh360.Api.Metier.Contracts.Notifications;

/// <summary>
/// Notification envoyée via SignalR.
/// </summary>
public sealed class NotificationsContractsNotificationDto
{
  public string Id { get; init; } = string.Empty;
  public string Type { get; init; } = "info";
  public string Message { get; init; } = string.Empty;

  public DateTimeOffset CreatedAt { get; init; }

  public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}
