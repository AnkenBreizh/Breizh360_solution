using System;

namespace Breizh360.Api.Metier.Contracts.Users;

/// <summary>
/// Version "liste" d'un user (exposition minimale).
/// </summary>
public sealed class UsersContractsUserSummaryDto
{
  public string Id { get; init; } = string.Empty;
  public string Login { get; init; } = string.Empty;
  public string Email { get; init; } = string.Empty;

  public bool IsActive { get; init; }

  public DateTimeOffset CreatedAt { get; init; }
}
