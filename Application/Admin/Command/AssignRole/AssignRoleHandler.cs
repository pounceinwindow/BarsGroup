using Application.Abstractions;
using Application.Abstractions.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Enums;
using System;

namespace Application.Admin.Command.AssignRole;

public class AssignRoleHandler : IRequestHandler<AssignRoleCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleHandler(IIdentityService identityService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        await _identityService.AssignRoleAsync(request.Username, request.RoleName);

        var identityUser = await _identityService.GetUserAsync(request.Username);
        if (identityUser == null) return;

        var dbRole = request.RoleName switch
        {
            "hr_platform_admin" => UserRole.Admin,
            "decider" => UserRole.Decider,
            _ => UserRole.HR
        };

        var user = await _userRepository.GetUserByUsernameAsync(request.Username, cancellationToken);
        if (user == null)
        {
            user = User.Create(
                request.Username,
                identityUser.FirstName,
                identityUser.LastName,
                null,
                dbRole,
                DateOnly.FromDateTime(DateTime.UtcNow),
                request.AssignedBy);
            await _userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            user.UpdateRole(dbRole, request.AssignedBy);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
