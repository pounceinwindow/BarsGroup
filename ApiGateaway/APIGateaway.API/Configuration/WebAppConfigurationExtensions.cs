using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

namespace APIGateaway.API.Configuration
{
    public static class WebAppConfigurationExtensions
    {
        public static IHostApplicationBuilder ConfigureDependencies(this IHostApplicationBuilder app)
        {
            return app
                .ConfigureHttpClient()
                .ConfigureRedisChache()
                .ConfigureAuthentication()
                .ConfigureReverseProxy();
        }

        private static IHostApplicationBuilder ConfigureRedisChache(this IHostApplicationBuilder app)
        {

            app.Services.AddStackExchangeRedisCache(conf =>
            {
                conf.Configuration = app.Configuration.GetConnectionString("Redis");
                conf.InstanceName = "APIGateway";
            });

            app.Services.AddDataProtection();

            app.Services.AddSingleton<ITicketStore, DistributedSessionStore>();

            return app;
        }

        private static IHostApplicationBuilder ConfigureAuthentication(this IHostApplicationBuilder app)
        {
            app.Services.AddAuthorization();

            app.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "Gateway.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = app.Configuration["Keycloak:Authority"];
                options.ClientId = app.Configuration["Keycloak:ClientId"];
                options.ClientSecret = app.Configuration["Keycloak:ClientSecret"];
                options.ResponseType = "code";

                options.RequireHttpsMetadata = false;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");

                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };

                options.CallbackPath = "/signin-oidc";
                options.SignedOutCallbackPath = "/signout-callback-oidc";
                options.RemoteSignOutPath = "/signout-oidc";

                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.TokenEndpointResponse != null && !string.IsNullOrEmpty(context.TokenEndpointResponse.ExpiresIn))
                        {
                            if (int.TryParse(context.TokenEndpointResponse.ExpiresIn, out int expiresInSeconds))
                            {
                                var expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
                                context.Properties?.UpdateTokenValue("expires_at", expiresAt.ToString("o"));
                            }
                        }
                        return Task.CompletedTask;
                    },

                    OnRedirectToIdentityProvider = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

            app.Services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .PostConfigure<ITicketStore>((options, store) =>
                {
                    options.SessionStore = store;
                });

            return app;
        }

        private static IHostApplicationBuilder ConfigureReverseProxy(this IHostApplicationBuilder app)
        {
            app.Services.AddReverseProxy()
            .LoadFromConfig(app.Configuration.GetSection("ReverseProxy"))
            .AddTransforms(transformContext =>
            {
                transformContext.AddRequestTransform(async requestContext =>
                {
                    var refreshService = requestContext.HttpContext.RequestServices.GetRequiredService<TokenRefreshService>();

                    var validToken = await refreshService.GetOrRefreshAccessTokenAsync(requestContext.HttpContext);

                    if (!string.IsNullOrEmpty(validToken))
                    {
                        requestContext.ProxyRequest.Headers.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", validToken);
                    }
                });
            });

            return app;
        }

        private static IHostApplicationBuilder ConfigureHttpClient(this IHostApplicationBuilder app)
        {
            app.Services.AddHttpClient();
            app.Services.AddScoped<TokenRefreshService>();

            return app;
        }
    }
}
