using Application.Abstractions;
using Application.Abstractions.Repositories;
using DoctorSite.Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.Decider
{
    public record CreateDeciderCommand(
        string Firstname,
        string Secondname,
        string Password) : IRequest<Result>;

    internal class CreateDeciderCommandHandler : IRequestHandler<CreateDeciderCommand, Result>
    {
        private readonly IKeycloackUserManager _userManager;
        private readonly IUserRepository _repo;
        private readonly ILogger<CreateDeciderCommandHandler> _logger;

        public CreateDeciderCommandHandler(
            IKeycloackUserManager userManager,
            IUserRepository repo,
            ILogger<CreateDeciderCommandHandler> logger)
        {
            _userManager = userManager;
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateDeciderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to create decider");
                
                //TODO User password hashing
                var user = User.Create(request.Firstname, request.Secondname, request.Password, Domain.Enums.UserRole.Decider, DateOnly.FromDateTime(DateTime.UtcNow));

                //Фамилия и имя не уникальны?
                await _userManager.CreateDecider($"{request.Firstname} {request.Secondname}", request.Password);

                //Если тут упадет неконсистентно

                await _repo.AddAsync(user, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while creating decider {Error}", ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}
