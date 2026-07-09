using Application.Admin.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions;

public interface IIdentityService
{
    Task<List<IdentityUserDto>> SearchUsersAsync(string query);
    Task AssignRoleAsync(string username, string roleName);
}
