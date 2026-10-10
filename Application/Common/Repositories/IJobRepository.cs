using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IJobRepository
    {
        Task AddAsync(Job job);
        Task<Job?> GetByIdAsync(Guid id);
        Task<Job?> GetWithDetailsAsync(Guid id);
        Task<PageResponse<Job>> SearchAsync(JobSearchFilter filter, PageRequest request, bool usePaging);
        Task<PageResponse<Job>> GetByRecruiterProfileIdAsync(
            Guid recruiterProfileId,
            JobPostingStatus? status,
            PageRequest request,
            bool usePaging);
        Task<PageResponse<Job>> GetAdminListAsync(AdminJobFilter filter, PageRequest pageRequest, bool usePaging);

        Task<Dictionary<WorkPlaceType, int>> GetWorkPlaceTypeCountsAsync(JobSearchFilter filter);
        Task<List<JobTitleCount>> GetTrendingJobTitlesAsync(List<Guid>? categoryIds, int take);
        Task<PageResponse<Job>> GetRecommendedAsync(
        List<Guid> skillIds,
        List<Guid> excludeJobIds,
        PageRequest request,
        bool usePaging);

        Task<Dictionary<JobPostingStatus, int>> GetStatusCountsByCompanyAsync(Guid companyId);
        Task<List<DateCountDto>> GetJobsCreatedTrendAsync(Guid companyId, DateTime start, DateTime end);
        Task<List<JobTitleCount>> GetTopJobTitlesByApplicationsAsync(Guid companyId, DateTime start, DateTime end, int take);
        Task<List<Guid>> GetJobIdsByCompanyAsync(Guid companyId);

        void Update(Job job);
        void Delete(Job job);

        public class DateCountDto
        {
            public DateTime Date { get; set; }
            public int Count { get; set; }
        }
    }

    public class JobSearchFilter
    {
        public string? Keyword { get; set; }
        public Guid? JobCategoryId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? Location { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public WorkPlaceType? WorkPlaceType { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public DateTime? PostedAfter { get; set; }
        public JobSortOption SortBy { get; set; } = JobSortOption.Newest;
    }

    public enum JobSortOption
    {
        Newest,
        SalaryHighToLow
    }

    public record JobTitleCount(string Title, int Count);
}