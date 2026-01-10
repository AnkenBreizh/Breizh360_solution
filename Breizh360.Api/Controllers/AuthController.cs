using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Breizh360.Api.Metier.Contracts.Auth;
using Breizh360.Api.Metier.Errors;
using Breizh360.Metier.Auth;
using Breizh360.Api.Services;
using Breizh360.Domaine.Common;

namespace Breizh360.Api.Metier.Controllers;

/// <summary>
/// Endpoints d’authentification. Cette implémentation appelle les services métier
/// pour valider les identifiants et émettre des tokens. Les erreurs sont
/// converties en objets <see cref="ErrorsApiError"/> afin d’offrir une réponse
/// homogène à l’API.
/// </summary>
[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthServiceValidateCredentials _authService;
    private readonly TokenService _tokenService;

    public AuthController(AuthServiceValidateCredentials authService, TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Login: renvoie access JWT + refresh token. Valide les identifiants via le service métier
    /// puis délègue l’émission des tokens au <see cref="TokenService"/>.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthContractsTokenPairResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status423Locked)]
    public async Task<ActionResult<AuthContractsTokenPairResponse>> Login([FromBody] AuthContractsLoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            // Retour standardisé des erreurs de validation (champ -> messages)
            var errors = new Dictionary<string, string[]>();
            foreach (var kv in ModelState)
            {
                if (kv.Value?.Errors.Count > 0)
                {
                    errors[kv.Key] = kv.Value.Errors.Select(e => e.ErrorMessage).ToArray();
                }
            }
            var err = new ErrorsApiError
            {
                Code = "INVALID_REQUEST",
                Message = "The request is invalid.",
                Status = StatusCodes.Status400BadRequest,
                TraceId = HttpContext.TraceIdentifier,
                CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v1) ? v1.ToString() : null,
                Errors = errors
            };
            return BadRequest(err);
        }

        try
        {
            var user = await _authService.ValidateOrThrowAsync(request.LoginOrEmail, request.Password, HttpContext.RequestAborted);
            var tokens = await _tokenService.IssueAsync(user);
            return Ok(tokens);
        }
        catch (AuthExceptionInvalidCredentials)
        {
            var err = new ErrorsApiError
            {
                Code = "INVALID_CREDENTIALS",
                Message = "Identifiants invalides.",
                Status = StatusCodes.Status401Unauthorized,
                TraceId = HttpContext.TraceIdentifier,
                CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v2) ? v2.ToString() : null
            };
            return StatusCode(StatusCodes.Status401Unauthorized, err);
        }
        catch (AuthExceptionUserLocked ex)
        {
            var err = new ErrorsApiError
            {
                Code = "USER_LOCKED",
                Message = $"Compte verrouillé jusqu’à {ex.LockedUntilUtc}.",
                Status = StatusCodes.Status423Locked,
                TraceId = HttpContext.TraceIdentifier,
                CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v3) ? v3.ToString() : null
            };
            return StatusCode(StatusCodes.Status423Locked, err);
        }
        catch (DomainException ex)
        {
            // Erreurs du domaine (ex: login invalide) -> 422 Unprocessable Entity
            var err = new ErrorsApiError
            {
                Code = "DOMAIN_ERROR",
                Message = ex.Message,
                Status = StatusCodes.Status422UnprocessableEntity,
                TraceId = HttpContext.TraceIdentifier,
                CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v4) ? v4.ToString() : null
            };
            return StatusCode(StatusCodes.Status422UnprocessableEntity, err);
        }
    }

    /// <summary>
    /// Refresh: rotation du refresh token. Un refresh valide génère un nouveau
    /// couple access/refresh token. Cette implémentation ne valide pas encore
    /// les refresh tokens et se contente d’en émettre un nouveau. La sécurité
    /// complète sera traitée ultérieurement.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthContractsTokenPairResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthContractsTokenPairResponse>> Refresh([FromBody] AuthContractsRefreshRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = new Dictionary<string, string[]>();
            foreach (var kv in ModelState)
            {
                if (kv.Value?.Errors.Count > 0)
                {
                    errors[kv.Key] = kv.Value.Errors.Select(e => e.ErrorMessage).ToArray();
                }
            }
            var err = new ErrorsApiError
            {
                Code = "INVALID_REQUEST",
                Message = "The request is invalid.",
                Status = StatusCodes.Status400BadRequest,
                TraceId = HttpContext.TraceIdentifier,
                CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v) ? v.ToString() : null,
                Errors = errors
            };
            return BadRequest(err);
        }

        // TODO: validate the refresh token once a persistent store exists
        var tokens = await _tokenService.RefreshAsync(request.RefreshToken);
        return Ok(tokens);
    }

    /// <summary>
    /// Logout (optionnel): révoque le refresh token courant. À implémenter
    /// lorsque la stratégie de stockage des refresh tokens sera définie.
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        // TODO: révoquer le refresh token dans le store.
        return NoContent();
    }
}