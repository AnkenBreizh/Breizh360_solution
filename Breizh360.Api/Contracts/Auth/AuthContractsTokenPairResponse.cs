namespace Breizh360.Api.Metier.Contracts.Auth;

public sealed class AuthContractsTokenPairResponse
{
  public string TokenType { get; init; } = "Bearer";
  public string AccessToken { get; init; } = string.Empty;
  public int ExpiresInSeconds { get; init; }
  public string RefreshToken { get; init; } = string.Empty;
}
