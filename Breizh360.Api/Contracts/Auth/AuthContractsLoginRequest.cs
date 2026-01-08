using System.ComponentModel.DataAnnotations;

namespace Breizh360.Api.Metier.Contracts.Auth;

public sealed class AuthContractsLoginRequest
{
  [Required]
  [MinLength(2)]
  public string LoginOrEmail { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  public string Password { get; set; } = string.Empty;
}
