using Application.Commands.Analytics;
using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetJobById
    {
        public record GetJobByIdQuery(Guid Id, Guid? ViewerUserId, ReferrerSource Referrer) : IRequest<Result<GetJobByIdResponse>>;

        public class GetJobByIdHandler(
            IJobRepository jobRepository,
            IMediator mediator) : IRequestHandler<GetJobByIdQuery, Result<GetJobByIdResponse>>
        {
            public async Task<Result<GetJobByIdResponse>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetWithDetailsAsync(request.Id);

                if (job is null)
                    return Result<GetJobByIdResponse>.Failure("Job not found");

                await mediator.Send(new LogAnalyticsEvent.LogAnalyticsEventCommand(
                     AnalyticsEventType.JobView,
                     AnalyticsSubjectType.Job,
                     job.Id,
                     request.ViewerUserId,
                     job.RecruiterProfile.UserId,
                     request.Referrer), cancellationToken);

                var response = new GetJobByIdResponse(
                    job.Id,
                    job.CompanyId,
                    job.Company.Name,
                    job.Company.LogoUrl,
                    job.RecruiterProfileId,
                    job.JobCategoryId,
                    job.Department.Name,
                    job.Title,
                    job.Description,
                    job.Requirement,
                    job.EmploymentType,
                    job.WorkPlaceType,
                    job.ExperienceLevel,
                    job.MinSalary,
                    job.MaxSalary,
                    job.Currency,
                    job.Location,
                    job.ApplicationDeadline,
                    job.Status,
                    job.DateCreated);

                return Result<GetJobByIdResponse>.Success(response, "Job retrieved successfully");
            }
        }
    }

    public record GetJobByIdResponse(
        Guid Id,
        Guid CompanyId,
        string CompanyName,
        string? CompanyLogoUrl,
        Guid RecruiterProfileId,
        Guid JobCategoryId,
        string CategoryName,
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
        DateTime DateCreated);
}