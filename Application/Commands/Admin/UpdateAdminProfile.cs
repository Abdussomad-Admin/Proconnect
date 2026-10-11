using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Admin
{
    public class UpdateAdminProfile
    {
        public record UpdateAdminProfileCommand(
            Guid UserId,
            string FirstName,
            string LastName,
            string? Tel,
            string? Bio,
            string? Location) : IRequest<Result<object>>;

        public class UpdateAdminProfileHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateAdminProfileCommand, Result<object>>
        {
            public async Task<Result<object>> Handle(UpdateAdminProfileCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                    return Result<object>.Failure("First name and last name are required");

                var user = await userRepository.GetByIdAsync(request.UserId);

                if (user is null)
                    return Result<object>.Failure("User not found");

                user.FirstName = request.FirstName.Trim();
                user.LastName = request.LastName.Trim();
                user.Tel = string.IsNullOrWhiteSpace(request.Tel) ? null : request.Tel.Trim();
                user.Bio = request.Bio?.Trim() ?? string.Empty;
                user.Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim();
                user.DateModified = DateTime.UtcNow;

                userRepository.UpdateAsync(user);
                await unitOfWork.SaveAsync();

                return Result<object>.Success(new { }, "Profile updated successfully");
            }
        }
    }
}