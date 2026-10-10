using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminUserDetails
    {
        public record GetAdminUserDetailsQuery(Guid UserId) : IRequest<Result<AdminUserDetailsResponse>>;

        public class GetAdminUserDetailsHandler(IUserRepository userRepository)
            : IRequestHandler<GetAdminUserDetailsQuery, Result<AdminUserDetailsResponse>>
        {
            public async Task<Result<AdminUserDetailsResponse>> Handle(
                GetAdminUserDetailsQuery request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdWithAdminDetailsAsync(request.UserId);

                if (user is null)
                    return Result<AdminUserDetailsResponse>.Failure("User not found");

                var response = new AdminUserDetailsResponse(
                    user.Id,
                    user.FirstName + " " + user.LastName,
                    user.UserName,
                    user.Email,
                    user.ProfilePictureUrl,
                    user.Location,
                    user.Bio,
                    user.ProfessionalProfile != null ? "Professional"
                        : user.RecruiterProfile != null ? "Recruiter" : "User",
                    user.Status.ToString(),
                    user.IsVerified,
                    user.DateCreated,
                    user.ProfessionalProfile == null ? null : new AdminProfessionalSummary(
                        user.ProfessionalProfile.HeadLine,
                        user.ProfessionalProfile.AvailabilityStatus.ToString(),
                        user.ProfessionalProfile.JobApplications.Count,
                        user.Posts.Count,
                        user.ProfessionalProfile.SavedJobs.Count),
                    user.RecruiterProfile == null ? null : new AdminRecruiterSummary(
                        user.RecruiterProfile.JobTitle,
                        user.RecruiterProfile.Company?.Name,
                        user.RecruiterProfile.CompanyId,
                        user.RecruiterProfile.Status.ToString(),
                        user.RecruiterProfile.Jobs.Count,
                        user.RecruiterProfile.Events.Count),
                    user.Following.Count,
                    user.Followers.Count,
                    user.ReportsReceived.Count);

                return Result<AdminUserDetailsResponse>.Success(response, "User details retrieved successfully");
            }
        }
    }

    public record AdminProfessionalSummary(
        string? Headline,
        string AvailabilityStatus,
        int JobsApplied,
        int PostsCreated,
        int SavedJobs);

    public record AdminRecruiterSummary(
        string? JobTitle,
        string? CompanyName,
        Guid? CompanyId,
        string RecruiterStatus,
        int JobsPosted,
        int EventsCreated);

    public record AdminUserDetailsResponse(
        Guid UserId,
        string Name,
        string UserName,
        string Email,
        string? AvatarUrl,
        string? Location,
        string Bio,
        string Type,
        string Status,
        bool IsVerified,
        DateTime JoinedAt,
        AdminProfessionalSummary? ProfessionalSummary,
        AdminRecruiterSummary? RecruiterSummary,
        int ConnectionsCount,
        int FollowersCount,
        int ReportsInvolvingUser);
}