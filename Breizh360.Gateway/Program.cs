// Breizh360.Gateway - skeleton (YARP)
// TODO: Add correlation id, centralized CORS, rate limiting, logging.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    // TODO: Load from ReverseProxyRoutes.json
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseHttpsRedirection();

// TODO: UseCorrelationIdMiddleware
// TODO: UseCors (centralized)
// TODO: UseRateLimiter

app.MapReverseProxy();

app.Run();
