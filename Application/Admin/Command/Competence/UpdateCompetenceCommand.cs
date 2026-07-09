using Application.Abstractions.Repositories;
using Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Command.Competence
{
    public record UpdateCompetenceCommand(
        int Id,
        string CompetenceName,
        string? Description) : IRequest<Result>;

    internal class UpdateCompetenceCommandHandler : IRequestHandler<UpdateCompetenceCommand, Result>
    {
        private readonly ICompetencyRepository _repo;
        private readonly ILogger<UpdateCompetenceCommandHandler> _logger;

        public UpdateCompetenceCommandHandler(
            ICompetencyRepository repo,
            ILogger<UpdateCompetenceCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateCompetenceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to update competence {CompetenceId}", request.Id);

                var competence = await _repo.GetByIdAsync(request.Id, cancellationToken);
                if (competence == null)
                    return Result.Failure("Competence not found");

                competence.Update(request.CompetenceName, request.Description);

                await _repo.UpdateAsync(competence, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while updating competence {CompetenceId} {Error}", request.Id, ex.Message);
                return Result.Failure(ex);
            }
        }
    }
}
