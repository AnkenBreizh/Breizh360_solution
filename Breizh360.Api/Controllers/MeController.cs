using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Breizh360.Api.Metier.Contracts.Me;
using Breizh360.Api.Metier.Errors;
using Breizh360.Domaine.Auth.Users;
using Breizh360.Domaine.Common;

namespace Breizh360.Api.Metier.Controllers;

/// <summary>
/// Endpoints exposant les informations du profil authentifié.
/// Récupère les informations du user courant à partir de ses claims et
/// interroge le dépôt des utilisateurs pour renvoyer un objet de contrat.
/// </summary>
[ApiController]
[Route("me")]
public sealed class MeController : ControllerBase
{
    private readonly IAuthUserRepository _userRepository;

    public MeController(IAuthUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Retourne le profil + rôles + permissions du user authentifié.
    /// </summary>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(MeContractsMeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MeContractsMeResponse>> Get()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            var err = new ErrorsApiError
            {
                Code = "UNAUTHORIZED",
                Message = "User identifier is missing.",
                Status = StatusCodes.Status401Unauthorized,
                TraceId = HttpContext.TraceIdentifier,
                CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v) ? v.ToString() : null
            };
            return StatusCode(StatusCodes.Status401Unauthorized, err);
        }

        var user = await _userRepository.GetByIdAsync(userId, HttpContext.RequestAborted);
        if (user is null)
        {
            var err = new ErrorsApiError
            {
                Code = "USER_NOT_FOUND",
                Message = "User not found.",
                Status = StatusCodes.Status401Unauthorized,
                TraceId = HttpContext.TraceIdentifier,
                CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v2) ? v2.ToString() : null
            };
            return StatusCode(StatusCodes.Status401Unauthorized, err);
        }

        // Récupération des rôles sous forme d’identifiants GUID (listés dans le dépôt). Pour exposer des noms, il
        // faudrait interroger le repository des rôles. Cela pourra être ajouté ultérieurement.
        var roleIds = await _userRepository.GetRoleIdsAsync(userId, HttpContext.RequestAborted);
        var roleStrings = roleIds.Select(id => id.ToString()).ToList();

        var response = new MeContractsMeResponse
        {
            UserId = user.Id.ToString(),
            Login = user.Login,
            Email = user.Email,
            Roles = roleStrings,
            Permissions = Array.Empty<string>()
        };

        return Ok(response);
    }
}