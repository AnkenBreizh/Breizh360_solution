using System.Text.Json.Serialization;

namespace Breizh360.Api.Metier.Errors;

public sealed class ErrorsApiError
{
  public string Code { get; init; } = "ERROR";
  public string Message { get; init; } = "An error occurred.";

  public int Status { get; init; }

  public string TraceId { get; init; } = string.Empty;
  public string? CorrelationId { get; init; }

  // Erreurs de validation : champ -> messages
  public Dictionary<string, string[]>? Errors { get; init; }

  // Extensibilité (ne pas casser les consommateurs)
  public string Version { get; init; } = "1";

  public static ErrorsApiError NotImplemented(HttpContext ctx, string code) => new()
  {
    Code = code,
    Message = "Not implemented.",
    Status = StatusCodes.Status501NotImplemented,
    TraceId = ctx.TraceIdentifier,
    CorrelationId = ctx.Response.Headers.TryGetValue("X-Correlation-ID", out var v) ? v.ToString() : null
  };
}
