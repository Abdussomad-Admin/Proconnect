using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Recruiter
{
    public class GetRecruiterPublicProfile
    {
        public record GetRecruiterPublicProfileQuery(Guid Id, Guid? ViewerUserId) : IRequest<Result<RecruiterPublicProfileResponse>>;

        public record RecruiterPublicProfileResponse(
            Guid Id,
            Guid UserId,
            string FullName,
            string? ProfilePictureUrl,
            string? Email,
            string? Tel,
            string? Location,
            string Bio,
            string? JobTitle,
            string? Department,
            bool IsCompanyAdmin,
            DateTime DateCreated,
            Guid? CompanyId,
            string? CompanyName,
            string? CompanyLogoUrl,
            string? CompanyIndustry,
            string? CompanyCompanySize
        );

        public class GetRecruiterPublicProfileHandler(
            IRecruiterProfileRepository recruiterProfileRepository)
            : IRequestHandler<GetRecruiterPublicProfileQuery, Result<RecruiterPublicProfileResponse>>
        {
            public async Task<Result<RecruiterPublicProfileResponse>> Handle(
                GetRecruiterPublicProfileQuery request,
                CancellationToken cancellationToken)
            {
                var profile = await recruiterProfileRepository.GetByIdAsync(request.Id);

                if (profile is null)
                {
                    return Result<RecruiterPublicProfileResponse>.Failure("Recruiter profile not found");
                }

                var result = new RecruiterPublicProfileResponse(
                    profile.Id,
                    profile.UserId,
                    $"{profile.User.FirstName} {profile.User.LastName}",
                    profile.User.ProfilePictureUrl,
                    profile.User.Email,
                    profile.User.Tel,
                    profile.User.Location,
                    profile.User.Bio,
                    profile.JobTitle,
                    profile.Department,
                    profile.IsCompanyAdmin,
                    profile.DateCreated,
                    profile.CompanyId,
                    profile.Company?.Name,
                    profile.Company?.LogoUrl,
                    profile.Company?.Industry,
                    profile.Company?.CompanySize);

                return Result<RecruiterPublicProfileResponse>.Success(result, "Recruiter public profile retrieved");
            }
        }
    }
}