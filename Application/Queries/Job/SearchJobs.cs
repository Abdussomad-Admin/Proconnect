using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class SearchJobs
    {
        public record SearchJobsQuery(
        string? Keyword,
        Guid? JobCategoryId,
        Guid? CompanyId,
        string? Location,
        EmploymentType? EmploymentType,
        WorkPlaceType? WorkPlaceType,
        ExperienceLevel? ExperienceLevel,
        decimal? MinSalary,
        decimal? MaxSalary,
        DateTime? PostedAfter,
        JobSortOption SortBy,
        PageRequest PageRequest,
        bool UsePaging
    ) : IRequest<Result<PageResponse<SearchJobsResponse>>>;

        public class SearchJobsHandler(
            IJobRepository jobRepository) : IRequestHandler<SearchJobsQuery, Result<PageResponse<SearchJobsResponse>>>
        {
            public async Task<Result<PageResponse<SearchJobsResponse>>> Handle(
                SearchJobsQuery request,
                CancellationToken cancellationToken)
            {
                var filter = new JobSearchFilter
                {
                    Keyword = request.Keyword,
                    JobCategoryId = request.JobCategoryId,
                    CompanyId = request.CompanyId,
                    Location = request.Location,
                    EmploymentType = request.EmploymentType,
                    WorkPlaceType = request.WorkPlaceType,
                    ExperienceLevel = request.ExperienceLevel,
                    MinSalary = request.MinSalary,
                    MaxSalary = request.MaxSalary,
                    PostedAfter = request.PostedAfter,
                    SortBy = request.SortBy
                };

                var jobs = await jobRepository.SearchAsync(filter, request.PageRequest, request.UsePaging);

                var response = new PageResponse<SearchJobsResponse>
                {
                    Items = jobs.Items.Select(j => new SearchJobsResponse(
                        j.Id,
                        j.CompanyId,
                        j.Company.Name,
                        j.Company.LogoUrl,
                        j.JobCategoryId,
                        j.Department.Name,
                        j.Title,
                        j.EmploymentType,
                        j.WorkPlaceType,
                        j.ExperienceLevel,
                        j.MinSalary,
                        j.MaxSalary,
                        j.Currency,
                        j.Location,
                        j.ApplicationDeadline,
                        j.DateCreated)).ToList(),
                    TotalCount = jobs.TotalCount,
                    PageNumber = jobs.PageNumber,
                    PageSize = jobs.PageSize
                };

                return Result<PageResponse<SearchJobsResponse>>.Success(response, "Jobs retrieved successfully");
            }
        }
    }

    public record SearchJobsResponse(
        Guid Id,
        Guid CompanyId,
        string CompanyName,
        string? CompanyLogoUrl,
        Guid JobCategoryId,
        string CategoryName,
        string Title,
        EmploymentType EmploymentType,
        WorkPlaceType WorkPlaceType,
        ExperienceLevel ExperienceLevel,
        decimal MinSalary,
        decimal MaxSalary,
        string Currency,
        string Location,
        DateTime ApplicationDeadline,
        DateTime DateCreated);
}