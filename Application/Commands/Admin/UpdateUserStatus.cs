using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Admin
{
    public class UpdateUserStatus
    {
        public record UpdateUserStatusCommand(
            Guid TargetUserId,
            Guid ActingAdminId,
            UserStatus NewStatus,
            string Action,
            string? Reason) : IRequest<Result<object>>;

        public class UpdateUserStatusHandler(
            IUserRepository userRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateUserStatusCommand, Result<object>>
        {
            public async Task<Result<object>> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdAsync(request.TargetUserId);

                if (user is null)
                    return Result<object>.Failure("User not found");

                if (user.Role.ToLower() == "admin")
                    return Result<object>.Failure("Admin accounts cannot be moderated");

                if (user.Status == request.NewStatus)
                    return Result<object>.Failure($"User is already {request.NewStatus}");

                var previousStatus = user.Status;
                user.Status = request.NewStatus;
                user.DateModified = DateTime.UtcNow;
                userRepository.UpdateAsync(user);

                await auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = request.ActingAdminId,
                    Action = request.Action,
                    Description = string.IsNullOrWhiteSpace(request.Reason)
                        ? $"Changed {user.UserName}'s status from {previousStatus} to {request.NewStatus}"
                        : $"Changed {user.UserName}'s status from {previousStatus} to {request.NewStatus}: {request.Reason}",
                    CreatedBy = request.ActingAdminId.ToString()
                });

                await unitOfWork.SaveAsync();

                return Result<object>.Success(new { }, $"User {request.Action.ToLower()}ed successfully");
            }
        }
    }
}