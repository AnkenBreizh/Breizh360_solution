namespace Breizh360.Gateway.Errors;

public sealed record GatewayError(
    string Code,
    string Message,
    string CorrelationId
);
