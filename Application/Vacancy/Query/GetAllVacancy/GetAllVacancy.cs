using Application.Abstractions.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Vacancy.Query.GetAllVacancy;

public record GetAllVacancyQuery()
    : IRequest<List<VacancyDto>>;

public class GetAllVacancyHandler
    : IRequestHandler<GetAllVacancyQuery, List<VacancyDto>>
{
    private IVacancyRepository vacancyRepository;
    public GetAllVacancyHandler(IVacancyRepository vacancyRepository)
    {
        this.vacancyRepository = vacancyRepository;
    }
    public Task<List<VacancyDto>> Handle(GetAllVacancyQuery request, CancellationToken cancellationToken)
    {
        return vacancyRepository.GetAll();
    }
}