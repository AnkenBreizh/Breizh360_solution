using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Breizh360.Api.Metier.Contracts.Notifications;

/// <summary>
/// Requête de test permettant d'émettre une notification vers l'utilisateur courant.
/// </summary>
public sealed class NotificationsContractsTestNotificationRequest
{
  [MaxLength(64)]
  public string Type { get; set; } = "info";

  [Required]
  [MaxLength(512)]
  public string Message { get; set; } = string.Empty;

  public Dictionary<string, string>? Metadata { get; set; }
}
