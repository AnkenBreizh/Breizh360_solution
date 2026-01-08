using System.Diagnostics;

namespace Breizh360.Api.Metier.Correlation;

public sealed class CorrelationCorrelationIdMiddleware
{
  public const string HeaderName = "X-Correlation-ID";

  private readonly RequestDelegate _next;
  private readonly ILogger<CorrelationCorrelationIdMiddleware> _logger;

  public CorrelationCorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationCorrelationIdMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task Invoke(HttpContext context)
  {
    var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var v) && !string.IsNullOrWhiteSpace(v)
      ? v.ToString()
      : Guid.NewGuid().ToString("N");

    context.Response.Headers[HeaderName] = correlationId;

    using (_logger.BeginScope(new Dictionary<string, object>
    {
      ["CorrelationId"] = correlationId,
      ["TraceId"] = context.TraceIdentifier
    }))
    {
      await _next(context);
    }
  }
}
