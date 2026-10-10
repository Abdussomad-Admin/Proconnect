using Application.Common.Dtos;
using Application.Common.Filters;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Recruiter
{
    public class GetSavedCandidates
    {
        public record SavedCandidateSkillResponse(Guid SkillId, string Name, string Level, int YearsOfExperience);

        public record SavedCandidateResponse(
            Guid SavedCandidateId,
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
            List<SavedCandidateSkillResponse> Skills,
            double MatchScorePercent,
            bool IsTopPick,
            DateTime SavedAt
        );

        public record GetSavedCandidatesQuery(
            Guid RequestingUserId,
            int PageNumber,
            int PageSize,
            bool UsePaging
        ) : IRequest<Result<PageResponse<SavedCandidateResponse>>>;

        public class GetSavedCandidatesHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            ISavedCandidateRepository savedCandidateRepository,
            IMatchScoreCalculator matchScoreCalculator)
            : IRequestHandler<GetSavedCandidatesQuery, Result<PageResponse<SavedCandidateResponse>>>
        {
            public async Task<Result<PageResponse<SavedCandidateResponse>>> Handle(
                GetSavedCandidatesQuery request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.RequestingUserId);

                if (recruiterProfile is null)
                    return Result<PageResponse<SavedCandidateResponse>>.Failure("You are not a recruiter");

                var pageRequest = new PageRequest { PageNumber = request.PageNumber, PageSize = request.PageSize };

                var page = await savedCandidateRepository.GetByRecruiterIdAsync(recruiterProfile.Id, pageRequest, request.UsePaging);

                var emptyFilter = new TalentSearchFilter();

                var items = page.Items.Select(sc =>
                {
                    var p = sc.ProfessionalProfile;

                    var totalYears = p.Experiences
                        .Where(e => !e.IsDeleted)
                        .Sum(e => ((e.IsCurrentJob ? DateTime.UtcNow : e.EndDate) - e.StartDate).Days / 365);

                    var education = p.Educations
                        .Where(e => !e.IsDeleted)
                        .OrderByDescending(e => e.EndDate ?? DateTime.MaxValue)
                        .FirstOrDefault();

                    var skills = p.ProfessionalSkills
                        .Where(ps => !ps.IsDeleted)
                        .Select(ps => new SavedCandidateSkillResponse(ps.SkillId, ps.Skill.Name, ps.Level, ps.YearsOfExperience))
                        .ToList();

                    var score = matchScoreCalculator.Calculate(p, emptyFilter, recruiterProfile.Company);

                    return new SavedCandidateResponse(
                        sc.Id,
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
                        score,
                        score >= 90,
                        sc.DateCreated
                    );
                }).ToList();

                var result = new PageResponse<SavedCandidateResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<SavedCandidateResponse>>.Success(result, "Saved candidates retrieved");
            }
        }
    }
}