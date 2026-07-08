using Application.Abstractions.Repositories;
using DoctorSite.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.Competence
{
    public record DeleteCompetenceCommand(
        int CompetenceId) : IRequest<Result>;

    internal class DeleteCompetenceCommandHandler : IRequestHandler<DeleteCompetenceCommand, Result>
    {
        private readonly ICompetentionRepository _repo;
        private readonly ILogger<DeleteCompetenceCommandHandler> _logger;

        public DeleteCompetenceCommandHandler(
            ICompetentionRepository repo,
            ILogger<DeleteCompetenceCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteCompetenceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to delete competence {CompetenceId}", request.CompetenceId);

                await _repo.DeleteByIdAsync(request.CompetenceId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while deleting competence {CompetenceId} {Error}", request.CompetenceId, ex.Message);

                return Result.Failure(ex);
            }
        }
    }
}
