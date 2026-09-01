using Microsoft.AspNetCore.Authentication;
using TrailShop.Api.Auth;
using TrailShop.Api.Middleware;
using TrailShop.Application;
using TrailShop.Infrastructure;
using TrailShop.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// TrailShop is deliberately vulnerable. Refuse to start anywhere except a local Development environment.
if (!builder.Environment.IsDevelopment())
{
    throw new InvalidOperationException(
        "TrailShop is a vulnerable-by-design training app. It only runs with ASPNETCORE_ENVIRONMENT=Development on a local machine.");
}

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(DemoHeaderAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, DemoHeaderAuthenticationHandler>(DemoHeaderAuthenticationHandler.SchemeName, _ => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(DemoHeaderAuthenticationHandler.AdminRole));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    db.Database.EnsureCreated();
    SeedData.EnsureSeeded(db);
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();

/// <summary>
/// Exposed for WebApplicationFactory in the integration tests.
/// </summary>
public partial class Program
{
}
