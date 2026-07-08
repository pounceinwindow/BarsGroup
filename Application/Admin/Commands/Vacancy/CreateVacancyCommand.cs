using Application.Abstractions.Repositories;
using DoctorSite.Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.Vacancy
{
    public record CreateVacancyCommand(
        string VacancyName) : IRequest<Result>;

    internal class CreateVacancyCommandHandler : IRequestHandler<CreateVacancyCommand, Result>
    {
        private readonly IVacancyRepository _repo;
        private readonly ILogger<CreateVacancyCommandHandler> _logger;

        public CreateVacancyCommandHandler(
            IVacancyRepository repo,
            ILogger<CreateVacancyCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateVacancyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to add new vacancy {VacancyName}", request.VacancyName);

                var vacancy = Domain.Models.Vacancy.Create(request.VacancyName);

                await _repo.AddAsync(vacancy, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while adding new vacancy {VacancyName} {Error}", request.VacancyName, ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}