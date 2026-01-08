using System.ComponentModel.DataAnnotations;

namespace Breizh360.Api.Metier.Contracts.Auth;

public sealed class AuthContractsRefreshRequest
{
  [Required]
  [MinLength(10)]
  public string RefreshToken { get; set; } = string.Empty;
}
