using Application.Abstractions.Repositories;
using Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Command.Vacancy
{
    public record UpdateVacancyCommand(
        int Id,
        string VacancyName) : IRequest<Result>;

    internal class UpdateVacancyCommandHandler : IRequestHandler<UpdateVacancyCommand, Result>
    {
        private readonly IVacancyRepository _repo;
        private readonly ILogger<UpdateVacancyCommandHandler> _logger;

        public UpdateVacancyCommandHandler(
            IVacancyRepository repo,
            ILogger<UpdateVacancyCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateVacancyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to update vacancy {VacancyId}", request.Id);

                var vacancy = await _repo.GetByIdAsync(request.Id, cancellationToken);
                if (vacancy == null)
                    return Result.Failure("Vacancy not found");

                vacancy.UpdateName(request.VacancyName);

                await _repo.UpdateAsync(vacancy, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while updating vacancy {VacancyId} {Error}", request.Id, ex.Message);
                return Result.Failure(ex);
            }
        }
    }
}
