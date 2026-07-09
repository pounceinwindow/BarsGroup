using Application.Abstractions.Repositories;
using Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Command.Vacancy
{
    public record AddVacancyCompetencyCommand(
        int VacancyId,
        int CompetencyId) : IRequest<Result>;

    internal class AddVacancyCompetencyCommandHandler : IRequestHandler<AddVacancyCompetencyCommand, Result>
    {
        private readonly IVacancyRepository _repo;
        private readonly ILogger<AddVacancyCompetencyCommandHandler> _logger;

        public AddVacancyCompetencyCommandHandler(
            IVacancyRepository repo,
            ILogger<AddVacancyCompetencyCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(AddVacancyCompetencyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to add competency to vacancy {VacancyId} {CompetenceId}", request.VacancyId, request.CompetencyId);

                await _repo.AddCompetencyByIdAsync(request.VacancyId, request.CompetencyId, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while adding competence to vacancy {VacancyId} {CompetenceId} {Error}",
                    request.VacancyId, request.CompetencyId, ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}
