using Application.Common.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IJobApplicationRepository
    {
        Task AddAsync(JobApplication application);
        Task<JobApplication?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid jobId, Guid professionalProfileId);
        Task<PageResponse<JobApplication>> GetByJobIdAsync(Guid jobId, PageRequest request, bool usePaging);
        Task<PageResponse<JobApplication>> GetByProfessionalProfileIdAsync(Guid professionalProfileId, PageRequest request, bool usePaging);
        Task<PageResponse<JobApplication>> GetByRecruiterProfileIdAsync(
            Guid recruiterProfileId,
            JobApplicationFilter filter,
            PageRequest request,
            bool usePaging);
        Task<Dictionary<JobStatus, int>> GetStatusCountsByRecruiterAsync(
            Guid recruiterProfileId,
            JobApplicationFilter filter);

        Task<Dictionary<Guid, Dictionary<JobStatus, int>>> GetStatusCountsGroupedByJobAsync(
            Guid companyId, DateTime start, DateTime end);

        Task<PageResponse<JobApplication>> GetByCompanyIdAsync(
            Guid companyId,
            JobApplicationFilter filter,
            PageRequest request,
            bool usePaging);
        Task<Dictionary<JobStatus, int>> GetStatusCountsByCompanyAsync(
            Guid companyId,
            JobApplicationFilter filter);

        Task<Dictionary<JobStatus, int>> GetStatusCountsByProfessionalAsync(Guid professionalProfileId);

        Task<int> GetCountInRangeByProfessionalAsync(Guid professionalProfileId, DateTime start, DateTime end);

        Task<List<DateCountDto>> GetApplicationsTrendByCompanyAsync(Guid companyId, DateTime start, DateTime end);
        Task<List<RecruiterPerformanceDto>> GetRecruiterPerformanceByCompanyAsync(Guid companyId, DateTime start, DateTime end);
        Task<InterviewStatsDto> GetInterviewStatsByCompanyAsync(Guid companyId, DateTime start, DateTime end);

        Task<(int Total, int Upcoming, int Past, int RejectedAfterScheduling, int DueTodayOrTomorrow)>
    GetInterviewCountsByRecruiterAsync(Guid recruiterProfileId);

        Task<(int CurrentPeriodCount, int PreviousPeriodCount)> GetInterviewSchedulingTrendByRecruiterAsync(
            Guid recruiterProfileId, DateTime periodStart, DateTime previousPeriodStart, DateTime previousPeriodEnd);

        void Update(JobApplication application);

        void Delete(JobApplication application);

        public class DateCountDto
        {
            public DateTime Date { get; set; }
            public int Count { get; set; }
        }

        public class RecruiterPerformanceDto
        {
            public Guid RecruiterProfileId { get; set; }
            public string FirstName { get; set; } = default!;
            public string LastName { get; set; } = default!;
            public int HiredCount { get; set; }
            public double AvgDaysToHire { get; set; }
        }

        public class InterviewStatsDto
        {
            public int Scheduled { get; set; }
            public int Completed { get; set; }
            public int Upcoming { get; set; }
        }
    }

    public class JobApplicationFilter
    {
        public JobStatus? Status { get; set; }
        public Guid? JobId { get; set; }
        public string? Keyword { get; set; }
        public DateTime? AppliedAfter { get; set; }
        public DateTime? AppliedBefore { get; set; }
        public WorkAuthorizationStatus? WorkAuthorization { get; set; }
    }
}