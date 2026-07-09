using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Command.Competence
{
    public record CreateCompetenceCommand(
        string CompetenceName,
        string? Description) : IRequest<Result>;

    internal class CreateCompetenceCommandHandler : IRequestHandler<CreateCompetenceCommand, Result>
    {
        private readonly ICompetencyRepository _repo;
        private readonly ILogger<CreateCompetenceCommandHandler> _logger;

        public CreateCompetenceCommandHandler(
            ICompetencyRepository repo,
            ILogger<CreateCompetenceCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateCompetenceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _repo.ExistsAsync(request.CompetenceName, cancellationToken))
                    return Result.Failure("The competence is already exist");

                _logger.LogInformation("Trying to create competence {CompetenceName}", request.CompetenceName);

                var competence = Competency.Create(request.CompetenceName, request.Description);

                await _repo.AddAsync(competence, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while creating competence {CompetenceName} {Error}", request.CompetenceName, ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}
