using System;
using System.Collections.Generic;

namespace Breizh360.Api.Metier.Contracts.Users;

/// <summary>
/// Version "détail" d'un user.
/// </summary>
public sealed class UsersContractsUserDetailsDto
{
  public string Id { get; init; } = string.Empty;
  public string Login { get; init; } = string.Empty;
  public string Email { get; init; } = string.Empty;

  public bool IsActive { get; init; }

  public DateTimeOffset CreatedAt { get; init; }
  public DateTimeOffset? UpdatedAt { get; init; }

  /// <summary>
  /// Liste des rôles (ids) - exposés en string pour éviter les soucis de sérialisation côté clients.
  /// </summary>
  public IReadOnlyList<string> RoleIds { get; init; } = Array.Empty<string>();
}
