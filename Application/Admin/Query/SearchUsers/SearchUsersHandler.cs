using Application.Abstractions;
using Application.Admin.DTO;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Admin.Query.SearchUsers;

public class SearchUsersHandler : IRequestHandler<SearchUsersQuery, List<IdentityUserDto>>
{
    private readonly IIdentityService _identityService;

    public SearchUsersHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<List<IdentityUserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.SearchUsersAsync(request.SearchTerm);
    }
}
