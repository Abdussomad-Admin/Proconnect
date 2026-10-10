using Application.Common.Dtos;
using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminJobs
    {
        public record GetAdminJobsQuery(
            string? Search,
            JobPostingStatus? Status,
            string? Industry,
            string? Location,
            DateTime? PostedFrom,
            DateTime? PostedTo,
            PageRequest PageRequest,
            bool UsePaging) : IRequest<Result<PageResponse<AdminJobListItem>>>;

        public class GetAdminJobsHandler(IJobRepository jobRepository)
            : IRequestHandler<GetAdminJobsQuery, Result<PageResponse<AdminJobListItem>>>
        {
            public async Task<Result<PageResponse<AdminJobListItem>>> Handle(
                GetAdminJobsQuery request, CancellationToken cancellationToken)
            {
                var filter = new AdminJobFilter(
                    request.Search, request.Status, request.Industry,
                    request.Location, request.PostedFrom, request.PostedTo);

                var jobs = await jobRepository.GetAdminListAsync(filter, request.PageRequest, request.UsePaging);

                var response = new PageResponse<AdminJobListItem>
                {
                    Items = jobs.Items.Select(j => new AdminJobListItem(
                        j.Id,
                        j.Title,
                        j.Department?.Name,
                        j.CompanyId,
                        j.Company.Name,
                        j.Company.LogoUrl,
                        j.Location,
                        j.EmploymentType.ToString(),
                        j.MinSalary,
                        j.MaxSalary,
                        j.Status.ToString(),
                        j.JobApplications.Count,
                        j.DateCreated)).ToList(),
                    TotalCount = jobs.TotalCount,
                    PageNumber = jobs.PageNumber,
                    PageSize = jobs.PageSize
                };

                return Result<PageResponse<AdminJobListItem>>.Success(response, "Jobs retrieved successfully");
            }
        }
    }

    public record AdminJobListItem(
        Guid JobId,
        string Title,
        string? Department,
        Guid CompanyId,
        string CompanyName,
        string? CompanyLogoUrl,
        string Location,
        string EmploymentType,
        decimal? MinSalary,
        decimal? MaxSalary,
        string Status,
        int ApplicationsCount,
        DateTime PostedDate);
}