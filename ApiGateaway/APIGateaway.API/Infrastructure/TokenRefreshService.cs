using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

public class TokenRefreshService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<CookieAuthenticationOptions> _cookieOptions;
    private readonly IConfiguration _configuration;

    public TokenRefreshService(
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<CookieAuthenticationOptions> cookieOptions,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _cookieOptions = cookieOptions;
        _configuration = configuration;
    }

    public async Task<string?> GetOrRefreshAccessTokenAsync(HttpContext context)
    {
        var authenticateResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded || authenticateResult.Properties == null)
        {
            return null;
        }

        var props = authenticateResult.Properties;
        var accessToken = props.GetTokenValue("access_token");
        var refreshToken = props.GetTokenValue("refresh_token");
        var expiresAtStr = props.GetTokenValue("expires_at");

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            return null;
        }

        if (!string.IsNullOrEmpty(expiresAtStr) &&
            DateTimeOffset.TryParse(expiresAtStr, CultureInfo.InvariantCulture, out var expiresAt) &&
            expiresAt > DateTimeOffset.UtcNow.AddSeconds(10))
        {
            return accessToken;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var authority = _configuration["Keycloak:Authority"];

            var tokenEndpoint = $"{authority?.TrimEnd('/')}/protocol/openid-connect/token";

            var requestParams = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", _configuration["Keycloak:ClientId"] ?? "" },
                { "client_secret", _configuration["Keycloak:ClientSecret"] ?? "" }
            };

            var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestParams));
            if (!response.IsSuccessStatusCode)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(
                KeycloakJsonContext.Default.KeycloakTokenResponse);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return null;
            }

            props.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                props.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
            }

            var newExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            props.UpdateTokenValue("expires_at", newExpiresAt.ToString("o", CultureInfo.InvariantCulture));

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticateResult.Principal!, props);

            return tokenResponse.AccessToken;
        }
        catch
        {
            return null;
        }
    }
}
public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
    [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
}

[JsonSerializable(typeof(KeycloakTokenResponse))]
internal partial class KeycloakJsonContext : JsonSerializerContext { }
