using Breizh360.Api.Metier.Correlation;
using Breizh360.Api.Metier.Errors;
using Breizh360.Api.Metier.Hubs;
using Breizh360.Api.Services;
using Breizh360.Data;
using Breizh360.Data.Auth.Seed;
using Breizh360.Data.Auth.Repositories;
using Breizh360.Domaine.Auth.Users;
using Breizh360.Metier.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Controllers + validation error contract
builder.Services
  .AddControllers()
  .ConfigureApiBehaviorOptions(options =>
  {
    options.InvalidModelStateResponseFactory = ErrorsInvalidModelStateFactory.Create;
  });

// --- Swagger (DEV uniquement dans le pipeline)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    const string schemeId = "Bearer";
    o.SwaggerDoc("v1", new() { Title = "Breizh360 API Métier", Version = "v1" });
    o.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT dans le header Authorization: Bearer {token}"
    });
    o.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(schemeId, document)] = new List<string>()
    });
});

// --- Db (PostgreSQL via EF Core)
builder.Services.AddDbContext<Breizh360DbContext>(opt =>
{
  var cs = builder.Configuration.GetConnectionString("Postgres");
  if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("Missing ConnectionStrings:Postgres");
  opt.UseNpgsql(cs);
});

// Register repositories and services
builder.Services.AddScoped<IAuthUserRepository, UserRepository>();
builder.Services.AddScoped<AuthServiceValidateCredentials>();
builder.Services.AddScoped<TokenService>();

// --- JWT validation (autorité = API)
var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection["SigningKey"];
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

if (string.IsNullOrWhiteSpace(signingKey) || signingKey.Length < 32)
  throw new InvalidOperationException("Jwt:SigningKey must be set and at least 32 chars.");
if (string.IsNullOrWhiteSpace(issuer))
  throw new InvalidOperationException("Jwt:Issuer must be set.");
if (string.IsNullOrWhiteSpace(audience))
  throw new InvalidOperationException("Jwt:Audience must be set.");

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new()
    {
      ValidateIssuer = true,
      ValidIssuer = issuer,
      ValidateAudience = true,
      ValidAudience = audience,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
      ValidateLifetime = true,
      ClockSkew = TimeSpan.FromSeconds(30),
      RoleClaimType = "role",
      NameClaimType = ClaimTypes.NameIdentifier
    };
    options.Events = new JwtBearerEvents
    {
      OnMessageReceived = context =>
      {
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;
        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
          context.Token = accessToken;
        return Task.CompletedTask;
      }
    };
  });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();
builder.Services.AddSignalR();

var app = builder.Build();

// Corrélation et erreurs homogènes
app.UseMiddleware<CorrelationCorrelationIdMiddleware>();
app.UseMiddleware<ErrorsExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  using var scope = app.Services.CreateScope();
  var db = scope.ServiceProvider.GetRequiredService<Breizh360DbContext>();
  await db.Database.MigrateAsync();
  await AuthSeedDev.EnsureSeedAsync(db);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();

// Expose Program class for integration tests.
public partial class Program { }