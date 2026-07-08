using Application.Abstractions.Repositories;
using DoctorSite.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.Vacancy
{
    public record DeleteVacancyCommand(
        int VacancyId) : IRequest<Result>;

    internal class DeleteVacancyCommandHandler : IRequestHandler<DeleteVacancyCommand, Result>
    {
        private readonly IVacancyRepository _repo;
        private readonly ILogger<DeleteVacancyCommandHandler> _logger;

        public DeleteVacancyCommandHandler(
            IVacancyRepository repo,
            ILogger<DeleteVacancyCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteVacancyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to delete vacancy {VacancyId}", request.VacancyId);

                await _repo.DeleteByIdAsync(request.VacancyId, cancellationToken);

                return Result.Success();
            }

            catch (Exception ex)
            {
                _logger.LogInformation("Error while deleting vacancy {VacancyId} {Error}", request.VacancyId, ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}

