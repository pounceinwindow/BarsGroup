using Application.Abstractions.Repositories;
using Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Command.Vacancy
{
    public record RemoveVacancyCompetencyCommand(
        int VacancyId,
        int CompetenceId) : IRequest<Result>;

    internal class RemoveVacancyCompetencyCommandHandler : IRequestHandler<RemoveVacancyCompetencyCommand, Result>
    {
        private readonly IVacancyRepository _repo;
        private readonly ILogger<RemoveVacancyCompetencyCommandHandler> _logger;

        public RemoveVacancyCompetencyCommandHandler(
            IVacancyRepository repo,
            ILogger<RemoveVacancyCompetencyCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveVacancyCompetencyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to delete competence from vacancy {VacancyId} {CompetenceId}", request.VacancyId, request.CompetenceId);

                await _repo.RemoveCompetencyByIdAsync(request.VacancyId, request.CompetenceId, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while deleting competence from vacancy {VacancyId} {CompetenceId} {Error}",
                    request.VacancyId, request.CompetenceId, ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}
