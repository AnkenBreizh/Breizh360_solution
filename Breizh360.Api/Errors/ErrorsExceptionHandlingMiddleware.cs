using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Breizh360.Api.Metier.Errors;

public sealed class ErrorsExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ErrorsExceptionHandlingMiddleware> _logger;

  public ErrorsExceptionHandlingMiddleware(RequestDelegate next, ILogger<ErrorsExceptionHandlingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task Invoke(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled exception");

      if (context.Response.HasStarted)
        throw;

      var status = StatusCodes.Status500InternalServerError;
      var code = "UNHANDLED";
      var message = "Unexpected error.";

      // Mapping minimal - à compléter selon les exceptions Domaine/Métier
      if (ex is UnauthorizedAccessException)
      {
        status = StatusCodes.Status401Unauthorized;
        code = "UNAUTHORIZED";
        message = "Unauthorized.";
      }

      var err = new ErrorsApiError
      {
        Code = code,
        Message = message,
        Status = status,
        TraceId = context.TraceIdentifier,
        CorrelationId = context.Response.Headers.TryGetValue("X-Correlation-ID", out var v) ? v.ToString() : null
      };

      context.Response.Clear();
      context.Response.StatusCode = status;
      context.Response.ContentType = MediaTypeNames.Application.Json;

      await context.Response.WriteAsync(JsonSerializer.Serialize(err));
    }
  }
}
