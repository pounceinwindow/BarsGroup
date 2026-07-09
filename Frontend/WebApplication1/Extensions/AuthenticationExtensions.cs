using System.Security.Claims;
using Domain.Enums;
using Domain.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication1.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Keycloak:Authority"] ?? "http://keycloak:8080/realms/hr-platform";
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
                        var db = context.HttpContext?.RequestServices.GetRequiredService<BarsContext>();
                        if (db == null) return;
                        
                        var username = context.Principal?.Identity?.Name;
                        if (!string.IsNullOrEmpty(username))
                        {
                            var identity = context.Principal?.Identity as ClaimsIdentity;
                            if (identity != null)
                            {
                                // Извлекаем роли из realm_access и добавляем как ClaimTypes.Role
                                var realmAccessClaim = context.Principal?.FindFirst("realm_access")?.Value;
                                if (!string.IsNullOrEmpty(realmAccessClaim))
                                {
                                    try
                                    {
                                        using var doc = System.Text.Json.JsonDocument.Parse(realmAccessClaim);
                                        if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                                        {
                                            foreach (var roleItem in rolesElement.EnumerateArray())
                                            {
                                                identity.AddClaim(new Claim(ClaimTypes.Role, roleItem.GetString() ?? string.Empty));
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }

                            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
                            if (user == null)
                            {
                                var firstName = context.Principal?.FindFirstValue(ClaimTypes.GivenName) ?? username;
                                var lastName = context.Principal?.FindFirstValue(ClaimTypes.Surname) ?? "Сотрудник";
                                
                                var role = UserRole.HR; // Дефолтная роль
                                user = User.Create(username, firstName, lastName, null, role, DateOnly.FromDateTime(DateTime.UtcNow));
                                db.Users.Add(user);
                                await db.SaveChangesAsync();
                            }

                            // Добавляем ID из базы как клейм, чтобы Blazor мог его использовать
                            identity?.AddClaim(new Claim("UserId", user.Id.ToString()));
                        }
                    }
                };
            });

        services.AddAuthorization();
        services.AddCascadingAuthenticationState();

        return services;
    }
}
