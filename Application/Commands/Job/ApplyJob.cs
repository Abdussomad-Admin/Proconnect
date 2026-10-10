using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class ApplyJob
    {
        public record ApplyJobCommand(
            Guid JobId,
            Guid ProfessionalProfileId,
            string CoverLetter,
            string? ResumeUrl,
            string? ResumePublicId,
            WorkAuthorizationStatus WorkAuthorization,
            string? AdditionalAnswers,
            string? ApplicantPhone,
            string? ApplicantLocation,
            string? ApplicantCurrentJobTitle,
            int? ApplicantYearsOfExperience,
            string? ApplicantLinkedInUrl,
            string CreatedBy
        ) : IRequest<Result<string>>;

        public class ApplyJobHandler(
            IJobRepository jobRepository,
            IJobApplicationRepository jobApplicationRepository,
            IProfessionalProfileRepository professionalProfileRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<ApplyJobCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                ApplyJobCommand request,
                CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<string>.Failure("Job not found");

                if (job.Status != JobPostingStatus.Active)
                    return Result<string>.Failure("This job is no longer accepting applications");

                if (job.ApplicationDeadline < DateTime.UtcNow)
                    return Result<string>.Failure("The application deadline for this job has passed");

                var alreadyApplied = await jobApplicationRepository.ExistsAsync(
                    request.JobId, request.ProfessionalProfileId);

                if (alreadyApplied)
                    return Result<string>.Failure("You have already applied to this job");

                var application = new Domain.Entities.JobApplication
                {
                    JobId = request.JobId,

                    ProfessionalProfileId = request.ProfessionalProfileId,

                    CoverLetter = request.CoverLetter,

                    ResumeUrl = request.ResumeUrl,

                    ResumePublicId = request.ResumePublicId,

                    WorkAuthorization = request.WorkAuthorization,

                    AdditionalAnswers = request.AdditionalAnswers,

                    ApplicantPhone = request.ApplicantPhone,

                    ApplicantLocation = request.ApplicantLocation,

                    ApplicantCurrentJobTitle = request.ApplicantCurrentJobTitle,

                    ApplicantYearsOfExperience = request.ApplicantYearsOfExperience,

                    ApplicantLinkedInUrl = request.ApplicantLinkedInUrl,

                    JobStatus = JobStatus.New,

                    AppliedAt = DateTime.UtcNow,

                    CreatedBy = request.CreatedBy
                };

                await jobApplicationRepository.AddAsync(application);

                await unitOfWork.SaveAsync();

                var recruiterProfile = await recruiterProfileRepository.GetByIdAsync(job.RecruiterProfileId);

                if (recruiterProfile is not null)
                {
                    var applicantProfile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                    var applicantName = applicantProfile is not null
                        ? $"{applicantProfile.User.FirstName} {applicantProfile.User.LastName}".Trim()
                        : "A candidate";

                    await notificationService.SendNotificationAsync(
                        recipientUserId: recruiterProfile.UserId,
                        actorUserId: applicantProfile?.UserId,
                        actorName: applicantName,
                        actorAvatarUrl: applicantProfile?.User.ProfilePictureUrl,
                        title: "New job application",
                        message: $"{applicantName} applied to your job posting \"{job.Title}\"",
                        type: NotificationType.JobApplication,
                        sourceEntityType: NotificationSourceEntityType.JobApplication,
                        sourceEntityId: application.Id,
                        actionUrl: $"/applications.html?jobId={job.Id}",
                        createdBy: request.CreatedBy);
                }

                return Result<string>.Success(application.Id.ToString(), "Application submitted successfully");
            }
        }
    }
}