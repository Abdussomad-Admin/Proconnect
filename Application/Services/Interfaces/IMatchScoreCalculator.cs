using Application.Common.Filters;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IMatchScoreCalculator
    {
        double Calculate(ProfessionalProfile candidate, TalentSearchFilter filter, Company? company);
    }

    public interface ILocationMatcher
    {
        double Score(string? candidateLocation, string? companyLocation);
    }
}