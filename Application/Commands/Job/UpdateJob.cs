using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class UpdateJob
    {
        public record UpdateJobCommand(
            Guid JobId,
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
            DateTime ApplicationDeadline
        ) : IRequest<Result<string>>;

        public class UpdateJobHandler(
            IJobRepository jobRepository,
            IDepartmentRepository departmentRepository,
            IJobApplicationRepository jobApplicationRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateJobCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateJobCommand request,
                CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<string>.Failure("Job not found");

                if (job.RecruiterProfileId != request.RecruiterProfileId)
                    return Result<string>.Failure("You are not authorized to edit this job");

                if (request.MinSalary > request.MaxSalary)
                    return Result<string>.Failure("Minimum salary cannot exceed maximum salary");

                var department = await departmentRepository.GetByIdAsync(request.JobCategoryId);

                if (department is null)
                    return Result<string>.Failure("Department not found");

                job.JobCategoryId = request.JobCategoryId;

                job.Title = request.Title;

                job.Description = request.Description;

                job.Requirement = request.Requirement;

                job.EmploymentType = request.EmploymentType;

                job.WorkPlaceType = request.WorkPlaceType;

                job.ExperienceLevel = request.ExperienceLevel;

                job.MinSalary = request.MinSalary;

                job.MaxSalary = request.MaxSalary;

                job.Currency = request.Currency;

                job.Location = request.Location;

                job.ApplicationDeadline = request.ApplicationDeadline;

                job.UpdatedAt = DateTime.UtcNow;

                jobRepository.Update(job);

                await unitOfWork.SaveAsync();

                var applicationsPage = await jobApplicationRepository.GetByJobIdAsync(
                    request.JobId, new PageRequest(), usePaging: false);

                foreach (var application in applicationsPage.Items)
                {
                    await notificationService.SendNotificationAsync(
                        recipientUserId: application.ProfessionalProfile.UserId,
                        actorUserId: null,
                        actorName: job.Company.Name,
                        actorAvatarUrl: job.Company.LogoUrl,
                        title: "Job details updated",
                        message: $"The job \"{job.Title}\" you applied to has been updated",
                        type: NotificationType.JobStatusUpdate,
                        sourceEntityType: NotificationSourceEntityType.JobApplication,
                        sourceEntityId: application.Id,
                        actionUrl: $"/job-detail.html?id={job.Id}",
                        createdBy: request.RecruiterProfileId.ToString());
                }

                return Result<string>.Success(job.Id.ToString(), "Job updated successfully");
            }
        }
    }
}