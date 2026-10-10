using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class PublishJob
    {
        public record PublishJobCommand(
            Guid JobId,
            Guid RecruiterProfileId
        ) : IRequest<Result<string>>;

        public class PublishJobHandler(
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PublishJobCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                PublishJobCommand request,
                CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<string>.Failure("Job not found");

                if (job.RecruiterProfileId != request.RecruiterProfileId)
                    return Result<string>.Failure("You are not authorized to publish this job");

                if (job.Status == JobPostingStatus.PendingApproval || job.Status == JobPostingStatus.Active)
                    return Result<string>.Failure("This job has already been submitted for review");

                job.Status = JobPostingStatus.PendingApproval;

                job.ScheduledPublishAt = null;

                job.IsActive = false;

                job.UpdatedAt = DateTime.UtcNow;

                jobRepository.Update(job);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(job.Id.ToString(), "Job submitted for review");
            }
        }
    }
}