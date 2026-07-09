using Application.Admin.DTO;
using MediatR;
using System.Collections.Generic;

namespace Application.Admin.Query.SearchUsers;

public record SearchUsersQuery(string SearchTerm) : IRequest<List<IdentityUserDto>>;
