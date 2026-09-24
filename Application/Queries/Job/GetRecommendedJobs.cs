using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetRecommendedJobs
    {
        public record GetRecommendedJobsQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<GetRecommendedJobsResponse>>>;

        public class GetRecommendedJobsHandler(
            IJobRepository jobRepository,
            IProfessionalSkillRepository professionalSkillRepository,
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetRecommendedJobsQuery, Result<PageResponse<GetRecommendedJobsResponse>>>
        {
            public async Task<Result<PageResponse<GetRecommendedJobsResponse>>> Handle(
                GetRecommendedJobsQuery request,
                CancellationToken cancellationToken)
            {
                var skillsPage = await professionalSkillRepository.GetByProfessionalProfileIdAsync(
                    request.PageRequest, usePaging: false, request.ProfessionalProfileId);
                var skillIds = skillsPage.Items.Select(s => s.SkillId).Distinct().ToList();

                var appliedPage = await jobApplicationRepository.GetByProfessionalProfileIdAsync(
                    request.ProfessionalProfileId, request.PageRequest, usePaging: false);
                var excludeJobIds = appliedPage.Items.Select(a => a.JobId).Distinct().ToList();

                bool isPersonalized = skillIds.Count > 0;

                PageResponse<Domain.Entities.Job> jobs;

                if (isPersonalized)
                {
                    jobs = await jobRepository.GetRecommendedAsync(
                        skillIds, excludeJobIds, request.PageRequest, request.UsePaging);

                    if (jobs.Items.Count() < 3)
                    {
                        var filter = new JobSearchFilter { SortBy = JobSortOption.Newest };
                        jobs = await jobRepository.SearchAsync(filter, request.PageRequest, request.UsePaging);
                        isPersonalized = false;
                    }
                }
                else
                {
                    var filter = new JobSearchFilter { SortBy = JobSortOption.Newest };
                    jobs = await jobRepository.SearchAsync(filter, request.PageRequest, request.UsePaging);
                }

                var response = new PageResponse<GetRecommendedJobsResponse>
                {
                    Items = jobs.Items.Select(j => new GetRecommendedJobsResponse(
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
                        j.DateCreated,
                        isPersonalized)).ToList(),
                    TotalCount = jobs.TotalCount,
                    PageNumber = jobs.PageNumber,
                    PageSize = jobs.PageSize
                };

                return Result<PageResponse<GetRecommendedJobsResponse>>.Success(response, "Recommended jobs retrieved successfully");
            }
        }
    }

    public record GetRecommendedJobsResponse(
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
        DateTime DateCreated,
        bool IsPersonalized);
}