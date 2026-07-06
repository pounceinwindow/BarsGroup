using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- НОВОЕ: Настройка менеджера ключей (JWKS) ---
// Этот объект автоматически скачает публичные ключи Keycloak и закеширует их.
var oidcConfigManager = new ConfigurationManager<OpenIdConnectConfiguration>(
    "http://localhost:8080/realms/hr-platform/.well-known/openid-configuration",
    new OpenIdConnectConfigurationRetriever(),
    new HttpDocumentRetriever { RequireHttps = false }
);
// Регистрируем его как Singleton, чтобы кэш ключей жил всё время работы приложения
builder.Services.AddSingleton(oidcConfigManager);


// Настраиваем хранение сессии в Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Gateway_Session";
        options.Cookie.HttpOnly = true; 
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; 
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// 1. Эндпоинт авторизации (вызывается из Postman)
app.MapPost("/api/login", async (LoginRequest req, HttpClient httpClient, HttpContext ctx) =>
{
    Console.WriteLine("\n================= [FLOW START] =================");
    Console.WriteLine($"[1] Получен запрос от Postman. User Info: логин '{req.Username}'");

    var tokenEndpoint = "http://localhost:8080/realms/hr-platform/protocol/openid-connect/token";

    var requestContent = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "password"),
        new KeyValuePair<string, string>("client_id", "api-gateway"),
        new KeyValuePair<string, string>("client_secret", "c8TS6haf56mLX87f8ee0PExlkYy1fSsG"), 
        new KeyValuePair<string, string>("username", req.Username),
        new KeyValuePair<string, string>("password", req.Password)
    });

    Console.WriteLine("[2] Отправляем User Info в Keycloak за токенами...");
    var response = await httpClient.PostAsync(tokenEndpoint, requestContent);

    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[ERROR] Ошибка от Keycloak: {response.StatusCode}. Детали: {error}");
        return Results.Unauthorized();
    }

    var tokenData = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

    Console.WriteLine("[3] Успех! Токены от Keycloak получены.");
    
    // --- НОВОЕ: Чтение и вывод ролей из токена ---
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(tokenData?.access_token);
    
    Console.WriteLine("[3.1] Бизнес-роли пользователя (из realm_access):");
    var realmAccess = jwtToken.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
    if (realmAccess != null)
    {
        using var doc = JsonDocument.Parse(realmAccess);
        if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
        {
            foreach (var role in rolesElement.EnumerateArray())
            {
                Console.WriteLine($"      - {role.GetString()}");
            }
        }
    }
    else
    {
        Console.WriteLine("      - Роли не найдены.");
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, req.Username),
        new Claim("access_token", tokenData?.access_token!),
        new Claim("refresh_token", tokenData?.refresh_token!)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    Console.WriteLine("[4] Формируем защищенную Cookie (Gateway_Session) и отправляем клиенту.");
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    Console.WriteLine("================== [FLOW END] ==================\n");

    return Results.Ok(new { message = "Логин успешен!" });
});


// 2. Тестовый защищенный эндпоинт (имитация проверки токена шлюзом/монолитом)
app.MapGet("/api/secure-data", async (HttpContext ctx, ConfigurationManager<OpenIdConnectConfiguration> configManager) =>
{
    var accessToken = ctx.User.FindFirst("access_token")?.Value;
    if (string.IsNullOrEmpty(accessToken)) return Results.Unauthorized();

    Console.WriteLine($"\n[GET /api/secure-data] Начинаем валидацию токена для '{ctx.User.Identity?.Name}'");

    try
    {
        // --- НОВОЕ: Криптографическая валидация токена ---
        // 1. Достаем публичные ключи из кэша (если их нет - менеджер сам сходит в Keycloak)
        var discoveryDocument = await configManager.GetConfigurationAsync();
        var publicKeys = discoveryDocument.SigningKeys;
        Console.WriteLine($"[1] Публичные ключи получены (Кол-во в кэше: {publicKeys.Count}).");

        // 2. Настраиваем правила проверки
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // Обязательно проверять математическую подпись!
            IssuerSigningKeys = publicKeys,  // Передаем публичные ключи
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8080/realms/hr-platform",
            ValidateAudience = false, // Зависит от настроек клиента (пока отключаем)
            ValidateLifetime = true,  // Проверка срока действия (exp)
            ClockSkew = TimeSpan.Zero // Убираем стандартную 5-минутную погрешность времени
        };

        // 3. Выполняем саму проверку. Если подпись не сойдется или токен протух - выбросится Exception
        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(accessToken, validationParameters, out var validatedToken);

        Console.WriteLine("[2] УСПЕХ: Подпись токена ВАЛИДНА! Токен не подделан.");
        Console.WriteLine("[3] Шлюз перенаправляет запрос в монолит...");
        
        return Results.Ok(new { message = "Доступ разрешен, криптографическая подпись проверена!" });
    }
    catch (SecurityTokenExpiredException)
    {
        Console.WriteLine("[ERROR] Токен протух (Expired). Здесь шлюз должен запустить фоновый Refresh.");
        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Токен не прошел математическую проверку подписи: {ex.Message}");
        return Results.Unauthorized();
    }

}).RequireAuthorization();

app.Run("http://localhost:5000");

// --- Модели данных ---
class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

class KeycloakTokenResponse
{
    public string access_token { get; set; } = string.Empty;
    public string refresh_token { get; set; } = string.Empty;
}