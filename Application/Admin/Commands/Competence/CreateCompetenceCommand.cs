using Application.Abstractions.Repositories;
using DoctorSite.Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Commands.Competence
{
    public record CreateCompetenceCommand(
        string CompetenceName,
        string? Description) : IRequest<Result>;

    internal class CreateCompetenceCommandHandler : IRequestHandler<CreateCompetenceCommand, Result>
    {
        private readonly ICompetentionRepository _repo;
        private readonly ILogger<CreateCompetenceCommandHandler> _logger;

        public CreateCompetenceCommandHandler(
            ICompetentionRepository repo,
            ILogger<CreateCompetenceCommandHandler> logger)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Result> Handle(CreateCompetenceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Trying to create competence {CompetenceName}", request.CompetenceName);

                var competence = Competency.Create(request.CompetenceName, request.Description);

                await _repo.AddAsync(competence);

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
