using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.Interview.Commands.CreateCandidate;

public class CreateCandidateHandler : IRequestHandler<CreateCandidateCommand, int>
{
    private readonly ICandidateRepository _candidates;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCandidateHandler(ICandidateRepository candidates, IUnitOfWork unitOfWork)
    {
        _candidates = candidates;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateCandidateCommand request, CancellationToken cancellationToken)
    {
        if (await _candidates.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("Candidate already exists.");
        }

        var candidate = Candidate.Create(
            request.FullName,
            request.Phone,
            request.Email,
            request.Telegram,
            request.City,
            request.Education,
            request.PreviousWork,
            request.Skills);

        await _candidates.AddAsync(candidate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return candidate.Id;
    }
}