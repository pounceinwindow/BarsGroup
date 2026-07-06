using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Настраиваем хранение сессии в Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Gateway_Session";
        options.Cookie.HttpOnly = true; 
        options.Cookie.SameSite = SameSiteMode.Strict;
        // Для Postman локально (без HTTPS) это нужно, иначе кука не прикрепится
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

    // URL для получения токенов (замени порт 8080 на свой, если отличается)
    var tokenEndpoint = "http://localhost:8080/realms/hr-platform/protocol/openid-connect/token";

    // Формируем payload для Keycloak (Direct Access Grants)
    var requestContent = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "password"),
        new KeyValuePair<string, string>("client_id", "api-gateway"),
        // Вставь сюда свой Client Secret из Keycloak
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
    Console.WriteLine($"    -> Access Token:  {tokenData?.access_token[..40]}... (обрезано)");
    Console.WriteLine($"    -> Refresh Token: {tokenData?.refresh_token[..40]}... (обрезано)");

    // Создаем сессию пользователя внутри шлюза.
    // Прячем токены внутрь Claims, чтобы шлюз мог достать их при следующих запросах.
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

    return Results.Ok(new { message = "Логин успешен! Посмотри вкладку Cookies в Postman." });
});

// 2. Тестовый защищенный эндпоинт (имитация похода в монолит)
app.MapGet("/api/secure-data", (HttpContext ctx) =>
{
    // Шлюз достает токен из зашифрованной Cookie
    var accessToken = ctx.User.FindFirst("access_token")?.Value;
    
    Console.WriteLine($"\n[GET /api/secure-data] Запрос с Cookie от '{ctx.User.Identity?.Name}'.");
    Console.WriteLine($"Шлюз достал токен из сессии: {accessToken?[..20]}...");
    Console.WriteLine("Здесь шлюз добавил бы этот токен в заголовок Authorization и проксировал запрос в монолит.");
    
    return Results.Ok(new { message = "Доступ разрешен!", user = ctx.User.Identity?.Name });
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