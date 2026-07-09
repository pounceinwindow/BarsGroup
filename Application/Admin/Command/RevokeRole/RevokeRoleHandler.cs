using Application.Abstractions;
using Application.Abstractions.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Application.Admin.Command.RevokeRole;

public class RevokeRoleHandler : IRequestHandler<RevokeRoleCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeRoleHandler(IIdentityService identityService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RevokeRoleCommand request, CancellationToken cancellationToken)
    {
        await _identityService.RevokeRolesAsync(request.Username);

        var user = await _userRepository.GetUserByUsernameAsync(request.Username, cancellationToken);
        if (user != null)
        {
            user.Revoke(DateOnly.FromDateTime(DateTime.UtcNow), request.RevokedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
