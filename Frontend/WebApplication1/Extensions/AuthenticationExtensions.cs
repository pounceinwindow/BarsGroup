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

        services.AddAuthorization();
        services.AddCascadingAuthenticationState();

        return services;
    }
}
