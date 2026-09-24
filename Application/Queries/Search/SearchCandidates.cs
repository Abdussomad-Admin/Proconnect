using Application.Common.Dtos;
using Application.Common.Filters;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Search
{
    public class SearchCandidates
    {
        public record CandidateSkillResponse(
            Guid SkillId,
            string Name,
            string Level,
            int YearsOfExperience
        );

        public record CandidateCardResponse(
            Guid ProfessionalProfileId,
            Guid UserId,
            string FullName,
            string? ProfilePictureUrl,
            string? HeadLine,
            AvailabilityStatus AvailabilityStatus,
            string? CurrentLocation,
            int TotalYearsExperience,
            string? HighestDegree,
            string? Institution,
            List<CandidateSkillResponse> Skills,
            double MatchScorePercent
        );

        public record SearchCandidatesQuery(
            Guid RequestingUserId,
            List<Guid>? SkillIds,
            int? MinYearsExperience,
            int? MaxYearsExperience,
            string? EducationLevel,
            string? Location,
            AvailabilityStatus? Availability,
            string? Keyword,
            int PageNumber,
            int PageSize,
            bool UsePaging
        ) : IRequest<Result<PageResponse<CandidateCardResponse>>>;

        public class SearchCandidatesHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IMatchScoreCalculator matchScoreCalculator)
            : IRequestHandler<SearchCandidatesQuery, Result<PageResponse<CandidateCardResponse>>>
        {
            public async Task<Result<PageResponse<CandidateCardResponse>>> Handle(
                SearchCandidatesQuery request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.RequestingUserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<PageResponse<CandidateCardResponse>>.Failure("You are not linked to a company");

                if (recruiterProfile.Status != RecruiterStatus.Active)
                    return Result<PageResponse<CandidateCardResponse>>.Failure("Your membership is pending");

                // ASSUMPTION: recruiterProfile.Company is loaded (navigation
                // included) by GetByUserIdAsync. If it's null here, the location
                // factor in match scoring silently drops out rather than failing
                // the whole search.
                var company = recruiterProfile.Company;

                var filter = new TalentSearchFilter
                {
                    SkillIds = request.SkillIds,
                    MinYearsExperience = request.MinYearsExperience,
                    MaxYearsExperience = request.MaxYearsExperience,
                    EducationLevel = request.EducationLevel,
                    Location = request.Location,
                    Availability = request.Availability,
                    Keyword = request.Keyword
                };

                var pageRequest = new PageRequest
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                var page = await professionalProfileRepository.SearchAsync(filter, pageRequest, request.UsePaging);

                var items = page.Items.Select(p =>
                {
                    var totalYears = p.Experiences
                        .Where(e => !e.IsDeleted)
                        .Sum(e => ((e.IsCurrentJob ? DateTime.UtcNow : e.EndDate) - e.StartDate).Days / 365);

                    var education = p.Educations
                        .Where(e => !e.IsDeleted)
                        .OrderByDescending(e => e.EndDate ?? DateTime.MaxValue)
                        .FirstOrDefault();

                    var skills = p.ProfessionalSkills
                        .Where(ps => !ps.IsDeleted)
                        .Select(ps => new CandidateSkillResponse(ps.SkillId, ps.Skill.Name, ps.Level, ps.YearsOfExperience))
                        .ToList();

                    return new CandidateCardResponse(
                        p.Id,
                        p.UserId,
                        $"{p.User.FirstName} {p.User.LastName}",
                        p.User.ProfilePictureUrl,
                        p.HeadLine,
                        p.AvailabilityStatus,
                        p.User.Location,
                        totalYears,
                        education?.Degree,
                        education?.Institution,
                        skills,
                        matchScoreCalculator.Calculate(p, filter, company)
                    );
                }).ToList();

                var result = new PageResponse<CandidateCardResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<CandidateCardResponse>>.Success(result, "Candidates retrieved");
            }
        }
    }
}