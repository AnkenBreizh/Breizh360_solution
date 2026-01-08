using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Breizh360.Api.Metier.Contracts.Me;
using Breizh360.Api.Metier.Errors;

namespace Breizh360.Api.Metier.Controllers;

[ApiController]
[Route("me")]
public sealed class MeController : ControllerBase
{
  /// <summary>Retourne le profil + rôles + permissions du user authentifié.</summary>
  [Authorize]
  [HttpGet]
  [ProducesResponseType(typeof(MeContractsMeResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
  public ActionResult<MeContractsMeResponse> Get()
  {
    // TODO (AUTH-API-002) : construire la réponse depuis les claims (role/perm) + infos user
    return StatusCode(StatusCodes.Status501NotImplemented, ErrorsApiError.NotImplemented(HttpContext, "ME_NOT_IMPLEMENTED"));
  }
}
