using Application.Abstractions.Repositories;
using Application.Abstractions;
using Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Command.Vacancy
{
    public record CreateVacancyCommand(
        string VacancyName,
        List<int> CompetencyIds) : IRequest<Result>;

    internal class CreateVacancyCommandHandler : IRequestHandler<CreateVacancyCommand, Result>
    {
        private readonly IVacancyRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateVacancyCommandHandler> _logger;

        public CreateVacancyCommandHandler(
            IVacancyRepository repo,
            IUnitOfWork unitOfWork,
            ILogger<CreateVacancyCommandHandler> logger)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateVacancyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to add new vacancy {VacancyName}", request.VacancyName);

                var vacancy = Domain.Models.Vacancy.Create(request.VacancyName);

                await _repo.AddAsync(vacancy, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                foreach (var competencyId in request.CompetencyIds)
                {
                    await _repo.AddCompetencyByIdAsync(vacancy.Id, competencyId, cancellationToken);
                }
                
                await _unitOfWork.SaveChangesAsync(cancellationToken);

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