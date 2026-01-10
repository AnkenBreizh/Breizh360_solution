using System.Collections.Generic;

namespace Breizh360.Api.Metier.Contracts.Users;

/// <summary>
/// Réponse paginée pour la liste des users.
/// </summary>
public sealed class UsersContractsUsersResponse
{
  public int Page { get; init; }
  public int PageSize { get; init; }
  public int TotalCount { get; init; }

  public IReadOnlyList<UsersContractsUserSummaryDto> Items { get; init; } = new List<UsersContractsUserSummaryDto>();
}
