using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminProfile
    {
        public record GetAdminProfileQuery(Guid UserId) : IRequest<Result<AdminProfileResponse>>;

        public class GetAdminProfileHandler(IUserRepository userRepository)
            : IRequestHandler<GetAdminProfileQuery, Result<AdminProfileResponse>>
        {
            public async Task<Result<AdminProfileResponse>> Handle(
                GetAdminProfileQuery request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdAsync(request.UserId);

                if (user is null)
                    return Result<AdminProfileResponse>.Failure("User not found");

                var response = new AdminProfileResponse(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.UserName,
                    user.Email,
                    user.Tel,
                    user.Bio,
                    user.Location,
                    user.ProfilePictureUrl,
                    user.IsVerified,
                    user.Status.ToString(),
                    user.DateCreated);

                return Result<AdminProfileResponse>.Success(response, "Profile retrieved successfully");
            }
        }
    }

    public record AdminProfileResponse(
        Guid UserId,
        string FirstName,
        string LastName,
        string UserName,
        string Email,
        string? Tel,
        string Bio,
        string? Location,
        string? ProfilePictureUrl,
        bool IsVerified,
        string Status,
        DateTime JoinedAt);
}