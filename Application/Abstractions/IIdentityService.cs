using Application.Admin.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions;

public interface IIdentityService
{
    Task<List<IdentityUserDto>> SearchUsersAsync(string query);
    Task<IdentityUserDto?> GetUserAsync(string username);
    Task AssignRoleAsync(string username, string roleName);
    Task RevokeRolesAsync(string username);
}
