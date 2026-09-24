using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Recruiter
{
    public class UpdateRecruiterProfile
    {
        public record UpdateRecruiterProfileCommand(
            Guid RequestingUserId,
            string FirstName,
            string LastName,
            string? JobTitle,
            string? Department,
            string? Tel,
            string? Location,
            string? Bio
        ) : IRequest<Result<Guid>>;

        public class UpdateRecruiterProfileHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<UpdateRecruiterProfileCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                UpdateRecruiterProfileCommand request,
                CancellationToken cancellationToken)
            {
                var profile = await recruiterProfileRepository.GetByUserIdAsync(request.RequestingUserId);

                if (profile is null)
                {
                    return Result<Guid>.Failure("Recruiter profile not found");
                }

                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                {
                    return Result<Guid>.Failure("First and last name are required");
                }

                profile.User.FirstName = request.FirstName;
                profile.User.LastName = request.LastName;
                profile.User.Tel = request.Tel;
                profile.User.Location = request.Location;
                profile.User.Bio = request.Bio;

                profile.JobTitle = request.JobTitle;
                profile.Department = request.Department;

                recruiterProfileRepository.UpdateAsync(profile);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = profile.UserId,
                        Action = "UpdateRecruiterProfile",
                        CreatedBy = profile.UserId.ToString(),
                        Description = $"Recruiter updated their own profile: {request.FirstName} {request.LastName}"
                    });

                await unitOfWork.SaveAsync();

                return Result<Guid>.Success(profile.Id, "Profile updated successfully");
            }
        }
    }
}