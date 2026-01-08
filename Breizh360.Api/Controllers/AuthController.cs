using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Breizh360.Api.Metier.Contracts.Auth;
using Breizh360.Api.Metier.Errors;

namespace Breizh360.Api.Metier.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
  /// <summary>Login: renvoie access JWT + refresh token.</summary>
  [HttpPost("login")]
  [ProducesResponseType(typeof(AuthContractsTokenPairResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
  public ActionResult<AuthContractsTokenPairResponse> Login([FromBody] AuthContractsLoginRequest request)
  {
    // TODO (AUTH-API-001) : appeler Breizh360.Metier.Auth.AuthServiceValidateCredentials + TokenService.IssueAsync
    return StatusCode(StatusCodes.Status501NotImplemented, ErrorsApiError.NotImplemented(HttpContext, "AUTH_LOGIN_NOT_IMPLEMENTED"));
  }

  /// <summary>Refresh: rotation du refresh token.</summary>
  [HttpPost("refresh")]
  [ProducesResponseType(typeof(AuthContractsTokenPairResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
  public ActionResult<AuthContractsTokenPairResponse> Refresh([FromBody] AuthContractsRefreshRequest request)
  {
    // TODO (AUTH-API-001) : appeler Breizh360.Metier.Auth.TokenService.RefreshAsync
    return StatusCode(StatusCodes.Status501NotImplemented, ErrorsApiError.NotImplemented(HttpContext, "AUTH_REFRESH_NOT_IMPLEMENTED"));
  }

  /// <summary>Logout (optionnel): révoque le refresh token courant.</summary>
  [Authorize]
  [HttpPost("logout")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
  public IActionResult Logout()
  {
    // TODO (AUTH-API-001) : révoquer refresh token (selon stratégie retenue)
    return NoContent();
  }
}
