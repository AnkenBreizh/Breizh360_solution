using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Breizh360.Api.Metier.Errors;

public static class ErrorsInvalidModelStateFactory
{
  public static IActionResult Create(ActionContext ctx)
  {
    var errors = ctx.ModelState
      .Where(kvp => kvp.Value?.Errors.Count > 0)
      .ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage).ToArray()
      );

    var err = new ErrorsApiError
    {
      Code = "VALIDATION",
      Message = "Validation failed.",
      Status = StatusCodes.Status400BadRequest,
      TraceId = ctx.HttpContext.TraceIdentifier,
      CorrelationId = ctx.HttpContext.Response.Headers.TryGetValue("X-Correlation-ID", out var v) ? v.ToString() : null,
      Errors = errors
    };

    return new ObjectResult(err)
    {
      StatusCode = StatusCodes.Status400BadRequest,
      ContentTypes = { MediaTypeNames.Application.Json }
    };
  }
}
