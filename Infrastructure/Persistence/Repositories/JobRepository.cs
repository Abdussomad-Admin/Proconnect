using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class JobRepository(ProConnectDbContext context) : IJobRepository
    {
        public async Task AddAsync(Job job)
        {
            await context.Jobs.AddAsync(job);
        }

        public async Task<Job?> GetByIdAsync(Guid id)
        {
            return await context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Department)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<Job?> GetWithDetailsAsync(Guid id)
        {
            return await context.Jobs
                .Include(j => j.Company)
                .Include(j => j.RecruiterProfile).ThenInclude(r => r.User)
                .Include(j => j.Department)
                .Include(j => j.JobApplications)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<PageResponse<Job>> GetAdminListAsync(AdminJobFilter filter, PageRequest pageRequest, bool usePaging)
        {
            var query = context.Jobs
                .AsNoTracking()
                .Include(j => j.Company)
                .Include(j => j.Department)
                .Include(j => j.JobApplications)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(j =>
                    j.Title.Contains(filter.Search) ||
                    j.Company.Name.Contains(filter.Search) ||
                    j.Location.Contains(filter.Search));

            if (filter.Status.HasValue)
                query = query.Where(j => j.Status == filter.Status.Value);

            if (!string.IsNullOrWhiteSpace(filter.Industry))
                query = query.Where(j => j.Company.Industry == filter.Industry);

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(j => j.Location.Contains(filter.Location));

            if (filter.PostedFrom.HasValue)
                query = query.Where(j => j.DateCreated >= filter.PostedFrom.Value);

            if (filter.PostedTo.HasValue)
                query = query.Where(j => j.DateCreated < filter.PostedTo.Value);

            query = query.OrderByDescending(j => j.DateCreated);

            if (usePaging)
            {
                var offset = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);

                return new PageResponse<Job>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<Job>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        private IQueryable<Job> ApplySearchFilter(JobSearchFilter filter, bool excludeWorkPlaceType = false)
        {
            var query = context.Jobs
                .AsNoTracking()
                .Include(j => j.Company)
                .Include(j => j.Department)
                .Where(j => j.Status == JobPostingStatus.Active)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                query = query.Where(j =>
                    j.Title.Contains(filter.Keyword) ||
                    j.Description.Contains(filter.Keyword));

            if (filter.JobCategoryId.HasValue)
                query = query.Where(j => j.JobCategoryId == filter.JobCategoryId.Value);

            if (filter.CompanyId.HasValue)
                query = query.Where(j => j.CompanyId == filter.CompanyId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(j => j.Location.Contains(filter.Location));

            if (filter.EmploymentType.HasValue)
                query = query.Where(j => j.EmploymentType == filter.EmploymentType.Value);

            if (!excludeWorkPlaceType && filter.WorkPlaceType.HasValue)
                query = query.Where(j => j.WorkPlaceType == filter.WorkPlaceType.Value);

            if (filter.ExperienceLevel.HasValue)
                query = query.Where(j => j.ExperienceLevel == filter.ExperienceLevel.Value);

            if (filter.MinSalary.HasValue)
                query = query.Where(j => j.MaxSalary >= filter.MinSalary.Value);

            if (filter.MaxSalary.HasValue)
                query = query.Where(j => j.MinSalary <= filter.MaxSalary.Value);

            if (filter.PostedAfter.HasValue)
                query = query.Where(j => j.DateCreated >= filter.PostedAfter.Value);

            return query;
        }

        public async Task<PageResponse<Job>> SearchAsync(
            JobSearchFilter filter,
            PageRequest request,
            bool usePaging)
        {
            var query = ApplySearchFilter(filter);

            query = filter.SortBy == JobSortOption.SalaryHighToLow
                ? query.OrderByDescending(j => j.MaxSalary)
                : query.OrderByDescending(j => j.DateCreated);

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<Job>
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

            return new PageResponse<Job>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<Dictionary<WorkPlaceType, int>> GetWorkPlaceTypeCountsAsync(JobSearchFilter filter)
        {
            var query = ApplySearchFilter(filter, excludeWorkPlaceType: true);

            return await query
                .GroupBy(j => j.WorkPlaceType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);
        }

        public async Task<List<JobTitleCount>> GetTrendingJobTitlesAsync(List<Guid>? categoryIds, int take)
        {
            var query = context.Jobs
                .AsNoTracking()
                .Where(j => j.Status == JobPostingStatus.Active)
                .AsQueryable();

            if (categoryIds is { Count: > 0 })
                query = query.Where(j => categoryIds.Contains(j.JobCategoryId));

            return await query
                .GroupBy(j => j.Title)
                .Select(g => new JobTitleCount(g.Key, g.Count()))
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync();
        }

        public async Task<PageResponse<Job>> GetByRecruiterProfileIdAsync(
            Guid recruiterProfileId,
            JobPostingStatus? status,
            PageRequest request,
            bool usePaging)
        {
            var query = context.Jobs
                .AsNoTracking()
                .Include(j => j.Department)
                .Include(j => j.JobApplications)
                .Where(j => j.RecruiterProfileId == recruiterProfileId)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(j => j.Status == status.Value);

            query = query.OrderByDescending(j => j.DateCreated);

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<Job>
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

            return new PageResponse<Job>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PageResponse<Job>> GetRecommendedAsync(
            List<Guid> skillIds,
            List<Guid> excludeJobIds,
            PageRequest request,
            bool usePaging)
        {
            var query = context.Jobs
                .AsNoTracking()
                .Include(j => j.Company)
                .Include(j => j.Department)
                .Where(j => j.Status == JobPostingStatus.Active)
                .Where(j => !excludeJobIds.Contains(j.Id))
                .Where(j => j.JobSkills.Any(js => skillIds.Contains(js.SkillId)))
                .Select(j => new
                {
                    Job = j,
                    MatchCount = j.JobSkills.Count(js => skillIds.Contains(js.SkillId))
                })
                .OrderByDescending(x => x.MatchCount)
                .ThenByDescending(x => x.Job.DateCreated);

            if (!usePaging)
            {
                var allItems = await query.Select(x => x.Job).ToListAsync();

                return new PageResponse<Job>
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
                .Select(x => x.Job)
                .ToListAsync();

            return new PageResponse<Job>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<Dictionary<JobPostingStatus, int>> GetStatusCountsByCompanyAsync(Guid companyId)
        {
            return await context.Jobs
                .AsNoTracking()
                .Where(j => j.CompanyId == companyId)
                .GroupBy(j => j.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<List<IJobRepository.DateCountDto>> GetJobsCreatedTrendAsync(Guid companyId, DateTime start, DateTime end)
        {
            return await context.Jobs
                .AsNoTracking()
                .Where(j => j.CompanyId == companyId
                    && j.DateCreated >= start && j.DateCreated <= end)
                .GroupBy(j => j.DateCreated.Date)
                .Select(g => new IJobRepository.DateCountDto { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<JobTitleCount>> GetTopJobTitlesByApplicationsAsync(
            Guid companyId, DateTime start, DateTime end, int take)
        {
            var rows = await context.JobApplications
                .AsNoTracking()
                .Where(a => a.Job.CompanyId == companyId
                    && a.AppliedAt >= start && a.AppliedAt <= end)
                .GroupBy(a => a.Job.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync();

            return rows.Select(r => new JobTitleCount(r.Title, r.Count)).ToList();
        }

        public async Task<List<Guid>> GetJobIdsByCompanyAsync(Guid companyId)
        {
            return await context.Jobs
                .AsNoTracking()
                .Where(j => j.CompanyId == companyId)
                .Select(j => j.Id)
                .ToListAsync();
        }

        public void Update(Job job)
        {
            context.Jobs.Update(job);
        }

        public void Delete(Job job)
        {
            context.Jobs.Remove(job);
        }
    }
}