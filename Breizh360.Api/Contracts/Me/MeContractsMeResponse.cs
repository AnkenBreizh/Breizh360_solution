namespace Breizh360.Api.Metier.Contracts.Me;

public sealed class MeContractsMeResponse
{
  public string UserId { get; init; } = string.Empty;
  public string Login { get; init; } = string.Empty;
  public string Email { get; init; } = string.Empty;

  public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
  public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();
}
