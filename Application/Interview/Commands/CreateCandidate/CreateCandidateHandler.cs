using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Enums;
using Domain.Models;
using MediatR;

namespace Application.Interview.Commands.CreateCandidate;

public class CreateCandidateHandler : IRequestHandler<CreateCandidateCommand, Unit>
{
    private readonly ICandidateRepository _candidates;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCandidateHandler(ICandidateRepository candidates, IUnitOfWork unitOfWork)
    {
        _candidates = candidates;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CreateCandidateCommand request, CancellationToken cancellationToken)
    {
        if (await _candidates.ExistsByEmailAsync(request.Email))
        {
            throw new ConflictException("Candidate already exists.");
        }

        var candidate = new Candidate
        {
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            Telegram = request.Telegram,
            City = request.City,
            Education = request.Education,
            PreviousWork = request.PreviousWork,
            Skills = request.Skills,
            Status = CandidateStatuses.LookingForWork
        };

        _candidates.Add(candidate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}