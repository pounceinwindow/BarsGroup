using Application.Abstractions.Repositories;
using DoctorSite.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.Vacancy
{
    public record AddVacancyCompetentionCommand(
        int VacancyId,
        int CompetentionId) : IRequest<Result>;

    internal class AddVacancyCompetentionCommandHandler : IRequestHandler<AddVacancyCompetentionCommand, Result>
    {
        private readonly IVacancyRepository _repo;
        private readonly ILogger<AddVacancyCompetentionCommandHandler> _logger;

        public AddVacancyCompetentionCommandHandler(
            IVacancyRepository repo,
            ILogger<AddVacancyCompetentionCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(AddVacancyCompetentionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to add competency to vacancy {VacancyId} {CompetenceId}", request.VacancyId, request.CompetentionId);

                await _repo.AddCompetencyByIdAsync(request.VacancyId, request.CompetentionId, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while adding competence to vacancy {VacancyId} {CompetenceId} {Error}",
                    request.VacancyId, request.CompetentionId, ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}
