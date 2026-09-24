using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.SavedCandidates
{
    public class GetSavedCandidates
    {
        public record SavedCandidateResponse(
            Guid SavedCandidateId,
            Guid ProfessionalProfileId,
            string FullName,
            string? ProfilePictureUrl,
            string? HeadLine,
            AvailabilityStatus AvailabilityStatus,
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
            ISavedCandidateRepository savedCandidateRepository)
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

                var items = page.Items.Select(sc => new SavedCandidateResponse(
                    sc.Id,
                    sc.ProfessionalProfileId,
                    $"{sc.ProfessionalProfile.User.FirstName} {sc.ProfessionalProfile.User.LastName}",
                    sc.ProfessionalProfile.User.ProfilePictureUrl,
                    sc.ProfessionalProfile.HeadLine,
                    sc.ProfessionalProfile.AvailabilityStatus,
                    sc.DateCreated
                )).ToList();

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