using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Admin
{
    public class UpdateJobStatus
    {
        public record UpdateJobStatusCommand(
            Guid JobId,
            Guid ActingAdminId,
            JobPostingStatus NewStatus,
            string Action,
            string? Reason) : IRequest<Result<object>>;

        public class UpdateJobStatusHandler(
            IJobRepository jobRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateJobStatusCommand, Result<object>>
        {
            public async Task<Result<object>> Handle(UpdateJobStatusCommand request, CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<object>.Failure("Job not found");

                if (job.Status == request.NewStatus)
                    return Result<object>.Failure($"Job is already {request.NewStatus}");

                var previousStatus = job.Status;
                job.Status = request.NewStatus;
                job.IsActive = request.NewStatus == JobPostingStatus.Active;
                job.UpdatedAt = DateTime.UtcNow;
                jobRepository.Update(job);

                await auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = request.ActingAdminId,
                    Action = request.Action,
                    Description = string.IsNullOrWhiteSpace(request.Reason)
                        ? $"Changed job '{job.Title}' status from {previousStatus} to {request.NewStatus}"
                        : $"Changed job '{job.Title}' status from {previousStatus} to {request.NewStatus}: {request.Reason}",
                    CreatedBy = request.ActingAdminId.ToString()
                });

                await unitOfWork.SaveAsync();

                return Result<object>.Success(new { }, $"Job {request.Action.ToLower()}ed successfully");
            }
        }
    }
}