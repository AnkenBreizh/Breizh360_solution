using Breizh360.Api.Metier.Contracts.Users;
using Breizh360.Api.Metier.Errors;
using Breizh360.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Breizh360.Api.Metier.Controllers;

/// <summary>
/// Endpoints de consultation des users.
/// MVP : endpoints read-only.
/// </summary>
[ApiController]
[Route("users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
  private readonly Breizh360DbContext _db;

  public UsersController(Breizh360DbContext db)
  {
    _db = db;
  }

  /// <summary>
  /// Liste paginée des users.
  /// </summary>
  /// <param name="page">Page (1..N). Défaut : 1.</param>
  /// <param name="pageSize">Taille (1..100). Défaut : 20.</param>
  /// <param name="q">Filtre texte (login ou email contient).</param>
  [HttpGet]
  [ProducesResponseType(typeof(UsersContractsUsersResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? q = null)
  {
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 1;
    if (pageSize > 100) pageSize = 100;

    var query = _db.Users.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(q))
    {
      var needle = q.Trim().ToLowerInvariant();
      query = query.Where(u =>
        u.Login.ToLower().Contains(needle) ||
        u.Email.ToLower().Contains(needle));
    }

    var total = await query.CountAsync(HttpContext.RequestAborted);

    var items = await query
      .OrderBy(u => u.Login)
      .ThenBy(u => u.Id)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Select(u => new UsersContractsUserSummaryDto
      {
        Id = u.Id.ToString(),
        Login = u.Login,
        Email = u.Email,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
      })
      .ToListAsync(HttpContext.RequestAborted);

    var response = new UsersContractsUsersResponse
    {
      Page = page,
      PageSize = pageSize,
      TotalCount = total,
      Items = items
    };

    return Ok(response);
  }

  /// <summary>
  /// Détail d'un user par id.
  /// </summary>
  [HttpGet("{id:guid}")]
  [ProducesResponseType(typeof(UsersContractsUserDetailsDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ErrorsApiError), StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> GetById(Guid id)
  {
    var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, HttpContext.RequestAborted);
    if (user is null)
    {
      var err = new ErrorsApiError
      {
        Code = "USER_NOT_FOUND",
        Message = "User not found.",
        Status = StatusCodes.Status404NotFound,
        TraceId = HttpContext.TraceIdentifier,
        CorrelationId = HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v) ? v.ToString() : null
      };
      return NotFound(err);
    }

    // Roles : lecture via table de jointure EF, sans exposer le modèle interne.
    var roleIds = await _db.UserRoles
      .AsNoTracking()
      .Where(ur => EF.Property<Guid>(ur, "UserId") == user.Id)
      .Select(ur => EF.Property<Guid>(ur, "RoleId"))
      .Distinct()
      .ToListAsync(HttpContext.RequestAborted);

    var dto = new UsersContractsUserDetailsDto
    {
      Id = user.Id.ToString(),
      Login = user.Login,
      Email = user.Email,
      IsActive = user.IsActive,
      CreatedAt = user.CreatedAt,
      UpdatedAt = user.UpdatedAt,
      RoleIds = roleIds.Select(r => r.ToString()).ToList()
    };

    return Ok(dto);
  }
}
