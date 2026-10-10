using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class CreateJob
    {
        public record CreateJobCommand(
            Guid CompanyId,
            Guid RecruiterProfileId,
            Guid JobCategoryId,
            string Title,
            string Description,
            string Requirement,
            EmploymentType EmploymentType,
            WorkPlaceType WorkPlaceType,
            ExperienceLevel ExperienceLevel,
            decimal MinSalary,
            decimal MaxSalary,
            string Currency,
            string Location,
            DateTime ApplicationDeadline,
            JobPostingStatus Status,
            DateTime? ScheduledPublishAt,
            string CreatedBy
        ) : IRequest<Result<string>>;

        public class CreateJobHandler(
            IJobRepository jobRepository,
            IDepartmentRepository jobCategoryRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateJobCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                CreateJobCommand request,
                CancellationToken cancellationToken)
            {
                if (request.Status is not (JobPostingStatus.Draft or JobPostingStatus.Scheduled))
                    return Result<string>.Failure("A job can only be created as a draft or scheduled");

                if (request.MinSalary > request.MaxSalary)
                    return Result<string>.Failure("Minimum salary cannot exceed maximum salary");

                if (request.ApplicationDeadline <= DateTime.UtcNow)
                    return Result<string>.Failure("Application deadline must be in the future");

                if (request.Status == JobPostingStatus.Scheduled && request.ScheduledPublishAt is null)
                    return Result<string>.Failure("A publish date is required when scheduling a job");

                if (request.Status == JobPostingStatus.Scheduled && request.ScheduledPublishAt <= DateTime.UtcNow)
                    return Result<string>.Failure("Scheduled publish date must be in the future");

                var category = await jobCategoryRepository.GetByIdAsync(request.JobCategoryId);

                if (category is null)
                    return Result<string>.Failure("Job category not found");

                var job = new Domain.Entities.Job
                {
                    CompanyId = request.CompanyId,

                    RecruiterProfileId = request.RecruiterProfileId,

                    JobCategoryId = request.JobCategoryId,

                    Title = request.Title,

                    Description = request.Description,

                    Requirement = request.Requirement,

                    EmploymentType = request.EmploymentType,

                    WorkPlaceType = request.WorkPlaceType,

                    ExperienceLevel = request.ExperienceLevel,

                    MinSalary = request.MinSalary,

                    MaxSalary = request.MaxSalary,

                    Currency = request.Currency,

                    Location = request.Location,

                    ApplicationDeadline = request.ApplicationDeadline,

                    Status = request.Status,

                    ScheduledPublishAt = request.Status == JobPostingStatus.Scheduled
                        ? request.ScheduledPublishAt
                        : null,

                    IsActive = request.Status == JobPostingStatus.Active,

                    CreatedBy = request.CreatedBy,

                    DateCreated = DateTime.UtcNow
                };

                await jobRepository.AddAsync(job);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(job.Id.ToString(), "Job created successfully");
            }
        }
    }
}