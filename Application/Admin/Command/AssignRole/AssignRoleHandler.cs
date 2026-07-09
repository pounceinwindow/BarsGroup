using Application.Abstractions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Admin.Command.AssignRole;

public class AssignRoleHandler : IRequestHandler<AssignRoleCommand>
{
    private readonly IIdentityService _identityService;

    public AssignRoleHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        await _identityService.AssignRoleAsync(request.Username, request.RoleName);
    }
}
