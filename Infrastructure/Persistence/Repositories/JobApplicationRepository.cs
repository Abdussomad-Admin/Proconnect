using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class JobApplicationRepository(ProConnectDbContext context) : IJobApplicationRepository
    {
        public async Task AddAsync(JobApplication application)
        {
            await context.JobApplications.AddAsync(application);
        }

        public async Task<JobApplication?> GetByIdAsync(Guid id)
        {
            return await context.JobApplications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.ProfessionalProfile)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> ExistsAsync(Guid jobId, Guid professionalProfileId)
        {
            return await context.JobApplications
                .AnyAsync(a => a.JobId == jobId && a.ProfessionalProfileId == professionalProfileId);
        }

        public async Task<Dictionary<Guid, Dictionary<JobStatus, int>>> GetStatusCountsGroupedByJobAsync(
            Guid companyId, DateTime start, DateTime end)
        {
            var rows = await context.JobApplications
                .AsNoTracking()
                .Where(a => a.Job.CompanyId == companyId
                    && a.AppliedAt >= start && a.AppliedAt <= end)
                .GroupBy(a => new { a.JobId, a.JobStatus })
                .Select(g => new { g.Key.JobId, g.Key.JobStatus, Count = g.Count() })
                .ToListAsync();

            return rows
                .GroupBy(r => r.JobId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(x => x.JobStatus, x => x.Count));
        }

        public async Task<PageResponse<JobApplication>> GetByJobIdAsync(
            Guid jobId,
            PageRequest request,
            bool usePaging)
        {
            var query = context.JobApplications
                .AsNoTracking()
                .Include(a => a.ProfessionalProfile)
                    .ThenInclude(p => p.User)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AppliedAt)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<JobApplication>
                {
                    Items = allItems,
                    TotalCount = allItems.Count,
                    PageNumber = 1,
                    PageSize = allItems.Count
                };
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<JobApplication>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<(int Total, int Upcoming, int Past, int RejectedAfterScheduling, int DueTodayOrTomorrow)>
    GetInterviewCountsByRecruiterAsync(Guid recruiterProfileId)
        {
            var now = DateTime.UtcNow;
            var tomorrowEnd = now.Date.AddDays(2);

            var baseQuery = context.JobApplications
                .AsNoTracking()
                .Where(a => a.Job.RecruiterProfileId == recruiterProfileId && a.InterviewScheduledAt != null);

            var total = await baseQuery.CountAsync();
            var upcoming = await baseQuery.CountAsync(a => a.InterviewScheduledAt > now);
            var past = await baseQuery.CountAsync(a => a.InterviewScheduledAt <= now && a.JobStatus != JobStatus.Rejected);
            var rejectedAfterScheduling = await baseQuery.CountAsync(a => a.JobStatus == JobStatus.Rejected);
            var dueTodayOrTomorrow = await baseQuery.CountAsync(a => a.InterviewScheduledAt > now && a.InterviewScheduledAt < tomorrowEnd);

            return (total, upcoming, past, rejectedAfterScheduling, dueTodayOrTomorrow);
        }

        public async Task<(int CurrentPeriodCount, int PreviousPeriodCount)> GetInterviewSchedulingTrendByRecruiterAsync(
            Guid recruiterProfileId, DateTime periodStart, DateTime previousPeriodStart, DateTime previousPeriodEnd)
        {
            var now = DateTime.UtcNow;

            var baseQuery = context.JobApplications
                .AsNoTracking()
                .Where(a => a.Job.RecruiterProfileId == recruiterProfileId && a.InterviewScheduledAt != null);

            var currentPeriodCount = await baseQuery.CountAsync(a =>
                a.InterviewScheduledAt >= periodStart && a.InterviewScheduledAt <= now);
            var previousPeriodCount = await baseQuery.CountAsync(a =>
                a.InterviewScheduledAt >= previousPeriodStart && a.InterviewScheduledAt < previousPeriodEnd);

            return (currentPeriodCount, previousPeriodCount);
        }

        public async Task<PageResponse<JobApplication>> GetByProfessionalProfileIdAsync(
            Guid professionalProfileId,
            PageRequest request,
            bool usePaging)
        {
            var query = context.JobApplications
                .AsNoTracking()
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Where(a => a.ProfessionalProfileId == professionalProfileId)
                .OrderByDescending(a => a.AppliedAt)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<JobApplication>
                {
                    Items = allItems,
                    TotalCount = allItems.Count,
                    PageNumber = 1,
                    PageSize = allItems.Count
                };
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<JobApplication>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private IQueryable<JobApplication> ApplyRecruiterFilter(
            Guid recruiterProfileId,
            JobApplicationFilter filter)
        {
            var query = context.JobApplications
                .AsNoTracking()
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.ProfessionalProfile)
                    .ThenInclude(p => p.User)
                .Where(a => a.Job.RecruiterProfileId == recruiterProfileId)
                .AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(a => a.JobStatus == filter.Status.Value);

            if (filter.JobId.HasValue)
                query = query.Where(a => a.JobId == filter.JobId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                query = query.Where(a =>
                    a.ProfessionalProfile.User.FirstName.Contains(filter.Keyword) ||
                    a.ProfessionalProfile.User.LastName.Contains(filter.Keyword) ||
                    a.ProfessionalProfile.User.Email.Contains(filter.Keyword) ||
                    a.Job.Title.Contains(filter.Keyword));

            if (filter.AppliedAfter.HasValue)
                query = query.Where(a => a.AppliedAt >= filter.AppliedAfter.Value);

            if (filter.AppliedBefore.HasValue)
                query = query.Where(a => a.AppliedAt <= filter.AppliedBefore.Value);

            if (filter.WorkAuthorization.HasValue)
                query = query.Where(a => a.WorkAuthorization == filter.WorkAuthorization.Value);

            return query;
        }

        public async Task<PageResponse<JobApplication>> GetByRecruiterProfileIdAsync(
            Guid recruiterProfileId,
            JobApplicationFilter filter,
            PageRequest request,
            bool usePaging)
        {
            var query = ApplyRecruiterFilter(recruiterProfileId, filter)
                .OrderByDescending(a => a.AppliedAt);

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<JobApplication>
                {
                    Items = allItems,
                    TotalCount = allItems.Count,
                    PageNumber = 1,
                    PageSize = allItems.Count
                };
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<JobApplication>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<Dictionary<JobStatus, int>> GetStatusCountsByRecruiterAsync(
            Guid recruiterProfileId,
            JobApplicationFilter filter)
        {
            var scopedFilter = new JobApplicationFilter
            {
                JobId = filter.JobId,
                Keyword = filter.Keyword,
                AppliedAfter = filter.AppliedAfter,
                AppliedBefore = filter.AppliedBefore,
                WorkAuthorization = filter.WorkAuthorization
            };

            var query = ApplyRecruiterFilter(recruiterProfileId, scopedFilter);

            return await query
                .GroupBy(a => a.JobStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        private IQueryable<JobApplication> ApplyCompanyFilter(
            Guid companyId,
            JobApplicationFilter filter)
        {
            var query = context.JobApplications
                .AsNoTracking()
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.ProfessionalProfile)
                    .ThenInclude(p => p.User)
                .Where(a => a.Job.CompanyId == companyId)
                .AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(a => a.JobStatus == filter.Status.Value);

            if (filter.JobId.HasValue)
                query = query.Where(a => a.JobId == filter.JobId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                query = query.Where(a =>
                    a.ProfessionalProfile.User.FirstName.Contains(filter.Keyword) ||
                    a.ProfessionalProfile.User.LastName.Contains(filter.Keyword) ||
                    a.ProfessionalProfile.User.Email.Contains(filter.Keyword) ||
                    a.Job.Title.Contains(filter.Keyword));

            if (filter.AppliedAfter.HasValue)
                query = query.Where(a => a.AppliedAt >= filter.AppliedAfter.Value);

            if (filter.AppliedBefore.HasValue)
                query = query.Where(a => a.AppliedAt <= filter.AppliedBefore.Value);

            if (filter.WorkAuthorization.HasValue)
                query = query.Where(a => a.WorkAuthorization == filter.WorkAuthorization.Value);

            return query;
        }

        public async Task<PageResponse<JobApplication>> GetByCompanyIdAsync(
            Guid companyId,
            JobApplicationFilter filter,
            PageRequest request,
            bool usePaging)
        {
            var query = ApplyCompanyFilter(companyId, filter)
                .OrderByDescending(a => a.AppliedAt);

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<JobApplication>
                {
                    Items = allItems,
                    TotalCount = allItems.Count,
                    PageNumber = 1,
                    PageSize = allItems.Count
                };
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<JobApplication>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<Dictionary<JobStatus, int>> GetStatusCountsByCompanyAsync(
            Guid companyId,
            JobApplicationFilter filter)
        {
            var scopedFilter = new JobApplicationFilter
            {
                JobId = filter.JobId,
                Keyword = filter.Keyword,
                AppliedAfter = filter.AppliedAfter,
                AppliedBefore = filter.AppliedBefore,
                WorkAuthorization = filter.WorkAuthorization
            };

            var query = ApplyCompanyFilter(companyId, scopedFilter);

            return await query
                .GroupBy(a => a.JobStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<Dictionary<JobStatus, int>> GetStatusCountsByProfessionalAsync(Guid professionalProfileId)
        {
            return await context.JobApplications
                .AsNoTracking()
                .Where(a => a.ProfessionalProfileId == professionalProfileId)
                .GroupBy(a => a.JobStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<int> GetCountInRangeByProfessionalAsync(Guid professionalProfileId, DateTime start, DateTime end)
        {
            return await context.JobApplications
                .AsNoTracking()
                .Where(a => a.ProfessionalProfileId == professionalProfileId
                    && a.AppliedAt >= start && a.AppliedAt <= end)
                .CountAsync();
        }

        public async Task<List<IJobApplicationRepository.DateCountDto>> GetApplicationsTrendByCompanyAsync(
            Guid companyId, DateTime start, DateTime end)
        {
            return await context.JobApplications
                .AsNoTracking()
                .Where(a => a.Job.CompanyId == companyId
                    && a.AppliedAt >= start && a.AppliedAt <= end)
                .GroupBy(a => a.AppliedAt.Date)
                .Select(g => new IJobApplicationRepository.DateCountDto { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<IJobApplicationRepository.RecruiterPerformanceDto>> GetRecruiterPerformanceByCompanyAsync(
    Guid companyId, DateTime start, DateTime end)
        {
            var rows = await context.JobApplications
                .AsNoTracking()
                .Include(a => a.LastActionedByRecruiterProfile)
                    .ThenInclude(r => r!.User)
                .Where(a => a.Job.CompanyId == companyId
                    && a.JobStatus == JobStatus.Hired
                    && a.LastActionedByRecruiterProfileId != null
                    && a.UpdatedAt >= start && a.UpdatedAt <= end)
                .Select(a => new
                {
                    RecruiterProfileId = a.LastActionedByRecruiterProfileId!.Value,
                    a.LastActionedByRecruiterProfile!.User.FirstName,
                    a.LastActionedByRecruiterProfile.User.LastName,
                    a.AppliedAt,
                    a.UpdatedAt
                })
                .ToListAsync();

            var hired = rows
                .Select(r => new
                {
                    r.RecruiterProfileId,
                    r.FirstName,
                    r.LastName,
                    DaysToHire = (r.UpdatedAt - r.AppliedAt).TotalDays
                })
                .ToList();

            return hired
                .GroupBy(h => new { h.RecruiterProfileId, h.FirstName, h.LastName })
                .Select(g => new IJobApplicationRepository.RecruiterPerformanceDto
                {
                    RecruiterProfileId = g.Key.RecruiterProfileId,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    HiredCount = g.Count(),
                    AvgDaysToHire = Math.Round(g.Average(x => x.DaysToHire), 1)
                })
                .OrderByDescending(x => x.HiredCount)
                .ToList();
        }

        public async Task<IJobApplicationRepository.InterviewStatsDto> GetInterviewStatsByCompanyAsync(
            Guid companyId, DateTime start, DateTime end)
        {
            var now = DateTime.UtcNow;

            var scheduled = await context.JobApplications
                .AsNoTracking()
                .Where(a => a.Job.CompanyId == companyId
                    && a.InterviewScheduledAt != null
                    && a.InterviewScheduledAt >= start && a.InterviewScheduledAt <= end)
                .CountAsync();

            var completed = await context.JobApplications
                .AsNoTracking()
                .Where(a => a.Job.CompanyId == companyId
                    && a.InterviewScheduledAt != null
                    && (a.JobStatus == JobStatus.Offered || a.JobStatus == JobStatus.Hired || a.JobStatus == JobStatus.Rejected)
                    && a.InterviewScheduledAt >= start && a.InterviewScheduledAt <= end)
                .CountAsync();

            var upcoming = await context.JobApplications
                .AsNoTracking()
                .Where(a => a.Job.CompanyId == companyId
                    && a.InterviewScheduledAt != null
                    && a.InterviewScheduledAt > now)
                .CountAsync();

            return new IJobApplicationRepository.InterviewStatsDto
            {
                Scheduled = scheduled,
                Completed = completed,
                Upcoming = upcoming
            };
        }

        public void Delete(JobApplication application)
        {
            context.JobApplications.Remove(application);
        }

        public void Update(JobApplication application)
        {
            context.JobApplications.Update(application);
        }
    }
}