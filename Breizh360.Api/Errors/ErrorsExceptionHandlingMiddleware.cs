using System;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Breizh360.Domaine.Common;
using Breizh360.Metier.Auth;

namespace Breizh360.Api.Metier.Errors;

/// <summary>
/// Middleware global de gestion des exceptions. Convertit les exceptions
/// métiers/domaine en <see cref="ErrorsApiError"/> avec le code HTTP
/// approprié et loggue toute erreur imprévue.
/// </summary>
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
            {
                throw;
            }

            var status = StatusCodes.Status500InternalServerError;
            var code = "UNHANDLED";
            var message = "Unexpected error.";

            switch (ex)
            {
                case UnauthorizedAccessException:
                    status = StatusCodes.Status401Unauthorized;
                    code = "UNAUTHORIZED";
                    message = "Unauthorized.";
                    break;
                case AuthExceptionInvalidCredentials:
                    status = StatusCodes.Status401Unauthorized;
                    code = "INVALID_CREDENTIALS";
                    message = "Identifiants invalides.";
                    break;
                case AuthExceptionUserLocked authLock:
                    status = StatusCodes.Status423Locked;
                    code = "USER_LOCKED";
                    message = $"Compte verrouillé jusqu’à {authLock.LockedUntilUtc}.";
                    break;
                case DomainException domainEx:
                    status = StatusCodes.Status422UnprocessableEntity;
                    code = "DOMAIN_ERROR";
                    message = domainEx.Message;
                    break;
                default:
                    // fallback to 500
                    break;
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