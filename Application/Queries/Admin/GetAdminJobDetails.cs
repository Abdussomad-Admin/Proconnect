using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminJobDetails
    {
        public record GetAdminJobDetailsQuery(Guid JobId) : IRequest<Result<AdminJobDetailsResponse>>;

        public class GetAdminJobDetailsHandler(IJobRepository jobRepository)
            : IRequestHandler<GetAdminJobDetailsQuery, Result<AdminJobDetailsResponse>>
        {
            public async Task<Result<AdminJobDetailsResponse>> Handle(
                GetAdminJobDetailsQuery request, CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetWithDetailsAsync(request.JobId);

                if (job is null)
                    return Result<AdminJobDetailsResponse>.Failure("Job not found");

                var response = new AdminJobDetailsResponse(
                    job.Id,
                    job.Title,
                    job.Description,
                    job.Department?.Name,
                    job.CompanyId,
                    job.Company.Name,
                    job.Company.LogoUrl,
                    job.Location,
                    job.EmploymentType.ToString(),
                    job.WorkPlaceType.ToString(),
                    job.ExperienceLevel.ToString(),
                    job.MinSalary,
                    job.MaxSalary,
                    job.Status.ToString(),
                    job.RecruiterProfile?.User.FirstName + " " + job.RecruiterProfile?.User.LastName,
                    job.JobApplications.Count,
                    job.DateCreated);

                return Result<AdminJobDetailsResponse>.Success(response, "Job details retrieved successfully");
            }
        }
    }

    public record AdminJobDetailsResponse(
        Guid JobId,
        string Title,
        string Description,
        string? Department,
        Guid CompanyId,
        string CompanyName,
        string? CompanyLogoUrl,
        string Location,
        string EmploymentType,
        string WorkPlaceType,
        string ExperienceLevel,
        decimal? MinSalary,
        decimal? MaxSalary,
        string Status,
        string? PostedByRecruiter,
        int ApplicationsCount,
        DateTime PostedDate);
}