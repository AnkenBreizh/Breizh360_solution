// Breizh360.Api - skeleton
// TODO: Wire DI (Application + Infrastructure), JWT auth, Swagger, SignalR, middlewares.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: AddAuthentication().AddJwtBearer(...)
// TODO: AddSignalR()
// TODO: AddApplication() / AddInfrastructure()

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// TODO: app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// TODO: app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();
