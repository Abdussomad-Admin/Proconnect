using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetApplicationsByRecruiter
    {
        public record GetApplicationsByRecruiterQuery(
            Guid RecruiterProfileId,
            JobStatus? Status,
            Guid? JobId,
            string? Keyword,
            DateTime? AppliedAfter,
            DateTime? AppliedBefore,
            WorkAuthorizationStatus? WorkAuthorization,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<GetApplicationsByRecruiterResponse>>>;

        public class GetApplicationsByRecruiterHandler(
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetApplicationsByRecruiterQuery, Result<PageResponse<GetApplicationsByRecruiterResponse>>>
        {
            public async Task<Result<PageResponse<GetApplicationsByRecruiterResponse>>> Handle(
                GetApplicationsByRecruiterQuery request,
                CancellationToken cancellationToken)
            {
                var filter = new JobApplicationFilter
                {
                    Status = request.Status,
                    JobId = request.JobId,
                    Keyword = request.Keyword,
                    AppliedAfter = request.AppliedAfter,
                    AppliedBefore = request.AppliedBefore,
                    WorkAuthorization = request.WorkAuthorization
                };

                var applications = await jobApplicationRepository.GetByRecruiterProfileIdAsync(
                    request.RecruiterProfileId, filter, request.PageRequest, request.UsePaging);

                var response = new PageResponse<GetApplicationsByRecruiterResponse>
                {
                    Items = applications.Items.Select(a => new GetApplicationsByRecruiterResponse(
                        a.Id,
                        a.ProfessionalProfileId,
                        a.ProfessionalProfile.User.FirstName,
                        a.ProfessionalProfile.User.LastName,
                        a.ProfessionalProfile.User.Email,
                        a.ProfessionalProfile.User.ProfilePictureUrl,
                        a.ProfessionalProfile.User.Location,
                        a.ProfessionalProfile.Summary,
                        a.JobId,
                        a.Job.Title,
                        a.Job.Company.Name,
                        a.CoverLetter,
                        a.ResumeUrl,
                        a.WorkAuthorization,
                        a.JobStatus,
                        a.InterviewScheduledAt,
                        a.InterviewType,
                        a.InterviewLocationOrLink,
                        a.AppliedAt,
                        a.UpdatedAt,
                        a.LastActionedByRecruiterProfileId)).ToList(),
                    TotalCount = applications.TotalCount,
                    PageNumber = applications.PageNumber,
                    PageSize = applications.PageSize
                };

                return Result<PageResponse<GetApplicationsByRecruiterResponse>>.Success(response, "Applications retrieved successfully");
            }
        }
    }

    public record GetApplicationsByRecruiterResponse(
        Guid ApplicationId,
        Guid ProfessionalProfileId,
        string FirstName,
        string LastName,
        string? Email,
        string? ProfilePictureUrl,
        string? Location,
        string? Summary,
        Guid JobId,
        string JobTitle,
        string CompanyName,
        string CoverLetter,
        string? ResumeUrl,
        WorkAuthorizationStatus WorkAuthorization,
        JobStatus JobStatus,
        DateTime? InterviewScheduledAt,
        string? InterviewType,
        string? InterviewLocationOrLink,
        DateTime AppliedAt,
        DateTime UpdatedAt,
        Guid? LastActionedByRecruiterProfileId);
}