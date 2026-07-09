using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Admin.DTO;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Identity;

public class KeycloakIdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly string _authority;
    private readonly string _realm;

    public KeycloakIdentityService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _authority = configuration["Keycloak:Authority"] ?? "http://keycloak:8080/realms/hr-platform";
        var uri = new Uri(_authority);
        _httpClient.BaseAddress = new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}");
        _realm = "hr-platform";
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/realms/master/protocol/openid-connect/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "password"},
            {"client_id", "admin-cli"},
            {"username", "admin"},
            {"password", "admin"}
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("access_token").GetString()!;
    }

    public async Task<List<IdentityUserDto>> SearchUsersAsync(string query)
    {
        var token = await GetAdminTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users?search={query}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
        return users?.Select(u => new IdentityUserDto
        {
            Id = u.Id,
            Username = u.Username,
            FirstName = u.FirstName ?? "",
            LastName = u.LastName ?? ""
        }).ToList() ?? new List<IdentityUserDto>();
    }

    public async Task AssignRoleAsync(string username, string roleName)
    {
        var token = await GetAdminTokenAsync();

        // 1. Get User
        var userRequest = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users?username={username}&exact=true");
        userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var userResponse = await _httpClient.SendAsync(userRequest);
        userResponse.EnsureSuccessStatusCode();
        var users = await userResponse.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
        var user = users?.FirstOrDefault();
        if (user == null) throw new Exception($"User {username} not found in Keycloak.");

        // 2. Get Role
        var roleRequest = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/roles/{roleName}");
        roleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var roleResponse = await _httpClient.SendAsync(roleRequest);
        roleResponse.EnsureSuccessStatusCode();
        var role = await roleResponse.Content.ReadFromJsonAsync<JsonElement>();

        // 3. Assign Role
        var assignRequest = new HttpRequestMessage(HttpMethod.Post, $"/admin/realms/{_realm}/users/{user.Id}/role-mappings/realm");
        assignRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        assignRequest.Content = JsonContent.Create(new[] { role });
        var assignResponse = await _httpClient.SendAsync(assignRequest);
        assignResponse.EnsureSuccessStatusCode();
    }
    public async Task<IdentityUserDto?> GetUserAsync(string username)
    {
        var token = await GetAdminTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users?username={username}&exact=true");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
        var user = users?.FirstOrDefault();

        if (user == null) return null;

        return new IdentityUserDto
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? ""
        };
    }

    public async Task RevokeRolesAsync(string username)
    {
        var token = await GetAdminTokenAsync();

        var userRequest = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users?username={username}&exact=true");
        userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var userResponse = await _httpClient.SendAsync(userRequest);
        userResponse.EnsureSuccessStatusCode();
        var users = await userResponse.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
        var user = users?.FirstOrDefault();
        if (user == null) return;

        // Get assigned realm roles
        var rolesRequest = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users/{user.Id}/role-mappings/realm");
        rolesRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var rolesResponse = await _httpClient.SendAsync(rolesRequest);
        rolesResponse.EnsureSuccessStatusCode();
        var roles = await rolesResponse.Content.ReadFromJsonAsync<JsonElement>();

        // Remove them
        var removeRequest = new HttpRequestMessage(HttpMethod.Delete, $"/admin/realms/{_realm}/users/{user.Id}/role-mappings/realm");
        removeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        removeRequest.Content = JsonContent.Create(roles);
        var removeResponse = await _httpClient.SendAsync(removeRequest);
        removeResponse.EnsureSuccessStatusCode();
    }
    private class KeycloakUserDto
    {
        [JsonPropertyName("id")] public string Id { get; set; } = null!;
        [JsonPropertyName("username")] public string Username { get; set; } = null!;
        [JsonPropertyName("firstName")] public string? FirstName { get; set; }
        [JsonPropertyName("lastName")] public string? LastName { get; set; }
    }
}
