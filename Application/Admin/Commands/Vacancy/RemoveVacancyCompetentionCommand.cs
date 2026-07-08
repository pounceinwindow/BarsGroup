using Application.Abstractions.Repositories;
using DoctorSite.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.Vacancy
{
    public record RemoveVacancyCompetentionCommand(
        int VacancyId,
        int CompetenceId) : IRequest<Result>;

    internal class RemoveVacancyCompetentionCommandHandler : IRequestHandler<RemoveVacancyCompetentionCommand, Result>
    {
        private readonly IVacancyRepository _repo;
        private readonly ILogger<RemoveVacancyCompetentionCommandHandler> _logger;

        public RemoveVacancyCompetentionCommandHandler(
            IVacancyRepository repo,
            ILogger<RemoveVacancyCompetentionCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveVacancyCompetentionCommand request, CancellationToken cancellationToken)
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
