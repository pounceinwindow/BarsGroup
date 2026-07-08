using System.Security.Claims;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Application;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Components;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddApplication();

builder.Services.AddScoped<SpectrumDemoState>();

// База данных
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"] ?? "http://keycloak:8080/realms/hr-platform";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            NameClaimType = "preferred_username"
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<BarsContext>();
                var username = context.Principal?.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
                    if (user == null)
                    {
                        var firstName = context.Principal?.FindFirstValue(ClaimTypes.GivenName) ?? username;
                        var lastName = context.Principal?.FindFirstValue(ClaimTypes.Surname) ?? "Сотрудник";
                        
                        var role = UserRole.HR; // Дефолтная роль
                        var newUser = User.Create(username, firstName, lastName, null, role, DateOnly.FromDateTime(DateTime.UtcNow));
                        db.Users.Add(newUser);
                        await db.SaveChangesAsync();
                    }
                }
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Применение миграций при запуске
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.BarsContext>();
    dbContext.Database.Migrate();
    if (!dbContext.Candidates.Any())
    {
        dbContext.ClearAndSeed().GetAwaiter().GetResult();
    }

    /*
     * "localhost/database" - для очистки и добавления тестовых данных в бд
     * [TODO: Андрей] - вынести это в Swagger
     */
}

app.Run();
