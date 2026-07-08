using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions;

public interface IIdentityService
{
    Task<List<IdentityUserDto>> SearchUsersAsync(string query);
    Task AssignRoleAsync(string username, string roleName);
}

public class IdentityUserDto
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
