using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries
{
    public class GetTalentInsights
    {
        public record TopSkillResponse(string SkillName, int CandidateCount);

        public record TalentInsightsResponse(
            int TotalCandidates,
            int NewThisWeek,
            double AverageMatchRatePercent,
            int HiresThisMonth,
            List<TopSkillResponse> TopSkills
        );

        public record GetTalentInsightsQuery(Guid RequestingUserId) : IRequest<Result<TalentInsightsResponse>>;

        public class GetTalentInsightsHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IProfessionalProfileRepository professionalProfileRepository)
            : IRequestHandler<GetTalentInsightsQuery, Result<TalentInsightsResponse>>
        {
            public async Task<Result<TalentInsightsResponse>> Handle(
                GetTalentInsightsQuery request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.RequestingUserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<TalentInsightsResponse>.Failure("You are not linked to a company");

                if (recruiterProfile.Status != RecruiterStatus.Active)
                    return Result<TalentInsightsResponse>.Failure("Your membership is pending");

                var stats = await professionalProfileRepository.GetTalentStatsAsync();

                var response = new TalentInsightsResponse(
                    stats.TotalCandidates,
                    stats.NewThisWeek,
                    stats.AverageMatchRatePercent,
                    stats.HiresThisMonth,
                    stats.TopSkills.Select(s => new TopSkillResponse(s.SkillName, s.CandidateCount)).ToList()
                );

                return Result<TalentInsightsResponse>.Success(response, "Talent insights retrieved");
            }
        }
    }
}