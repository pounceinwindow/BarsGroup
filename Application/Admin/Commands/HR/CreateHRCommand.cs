using Application.Abstractions;
using Application.Abstractions.Repositories;
using DoctorSite.Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.HR
{
    public record CreateHRCommand(
        string FirstName,
        string Lastname,
        string Password) : IRequest<Result>;

    internal class CreateHRCommandHandler : IRequestHandler<CreateHRCommand, Result>
    {
        private readonly IKeycloackUserManager _userManager;
        private readonly IUserRepository _repo;
        private readonly ILogger<CreateHRCommandHandler> _logger;

        public CreateHRCommandHandler(
            IKeycloackUserManager userManager,
            IUserRepository repo,
            ILogger<CreateHRCommandHandler> logger)
        {
            _userManager = userManager;
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateHRCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to create new HR");

                //TODO А если имя и фамилия неуникальны?
                await _userManager.CreateHR($"{request.FirstName} {request.Lastname}", request.Password);

                //Если тут или дальше прервется то несогласованность будет

                //TODO хэш пароля
                var user = User.Create(request.FirstName, request.Lastname, request.Password, Domain.Enums.UserRole.HR, DateOnly.FromDateTime(DateTime.UtcNow));

                await _repo.AddAsync(user);

                return Result.Success();
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error while creating new HR {Error}", ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}
