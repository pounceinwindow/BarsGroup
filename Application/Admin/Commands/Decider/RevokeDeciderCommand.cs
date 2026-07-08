using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using DoctorSite.Application.Common;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.Decider
{
    public record RevokeDeciderCommand(
        int Id) : IRequest<Result>;

    internal class RevokeDeciderCommandHandler : IRequestHandler<RevokeDeciderCommand, Result>
    {
        private readonly IKeycloackUserManager _userManager;
        private readonly IUserRepository _repo;
        private readonly ILogger<RevokeDeciderCommandHandler> _logger;

        public RevokeDeciderCommandHandler(
            IKeycloackUserManager userManager,
            IUserRepository repo,
            ILogger<RevokeDeciderCommandHandler> logger)
        {
            _userManager = userManager;
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(RevokeDeciderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to revoke decider {DeciderId}", request.Id);

                var user = await _repo.GetUserAsync(request.Id, cancellationToken);

                if (!(user.Role == UserRole.Decider))
                    return Result.Failure(new DomainError("The user is not HR"));

                if (!(user.RevokedAt == null))
                    return Result.Failure(new DomainError("The user is already revoked"));

                //Опять вопрос с уникальностью пары имя фамилия
                await _userManager.RevokeHR($"{user.FirstName} {user.LastName}");

                //Если тут или дальше упадет то несогласованность будет

                await _repo.MarkRevokedAsync(user.Id, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while revoking user {UserId} {Error}", request.Id, ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}
