using Application.Common.Dtos;
using Application.Common.Filters;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProfessionalProfileRepository(ProConnectDbContext context) : IProfessionalProfileRepository
    {

        public async Task AddAsync(ProfessionalProfile profile)
        {
            await context.ProfessionalProfiles.AddAsync(profile);
        }

        public async Task<ProfessionalProfile?> GetByIdAsync(Guid id)
        {
            return await context.ProfessionalProfiles
                .Include(p => p.User)
                .Include(p => p.Educations)
                .Include(p => p.Experiences)
                .Include(p => p.Certificates)
                .Include(p => p.Projects)
                .Include(p => p.PortfolioLinks)
                .Include(p => p.ProfessionalSkills)
                    .ThenInclude(ps => ps.Skill)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId)
        {
            return await context.ProfessionalProfiles
                .Include(u => u.User)
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);
        }

        public async Task<ProfessionalProfile?> GetWithDetailsAsync(Guid id)
        {
            return await context.ProfessionalProfiles
                .Include(p => p.User)
                .Include(p => p.Educations)
                .Include(p => p.Experiences)
                .Include(p => p.Certificates)
                .Include(p => p.Projects)
                .Include(p => p.PortfolioLinks)
                .Include(p => p.ProfessionalSkills)
                    .ThenInclude(ps => ps.Skill)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<PageResponse<ProfessionalProfile>> GetAllAsync(
            PageRequest request,
            bool usePaging)
        {
            var query = context.ProfessionalProfiles
                .AsNoTracking()
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<ProfessionalProfile>
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

            return new PageResponse<ProfessionalProfile>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Update(ProfessionalProfile profile)
        {
            context.ProfessionalProfiles.Update(profile);
        }

        public void Delete(ProfessionalProfile profile)
        {
            context.ProfessionalProfiles.Remove(profile);
        }

        // ---------------- Talent Search additions ----------------

        public async Task<PageResponse<ProfessionalProfile>> SearchAsync(
    TalentSearchFilter filter,
    PageRequest request,
    bool usePaging)
        {
            var query = context.ProfessionalProfiles
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Experiences)
                .Include(p => p.Educations)
                .Include(p => p.ProfessionalSkills)
                    .ThenInclude(ps => ps.Skill)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (filter.SkillIds is { Count: > 0 })
            {
                query = query.Where(p => p.ProfessionalSkills
                    .Any(ps => !ps.IsDeleted && filter.SkillIds.Contains(ps.SkillId)));
            }

            if (filter.Availability.HasValue)
            {
                query = query.Where(p => p.AvailabilityStatus == filter.Availability.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();
                query = query.Where(p =>
                    (p.HeadLine != null && p.HeadLine.Contains(keyword)) ||
                    p.User.FirstName.Contains(keyword) ||
                    p.User.LastName.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(filter.EducationLevel))
            {
                query = query.Where(p => p.Educations
                    .Any(e => !e.IsDeleted && e.Degree.Contains(filter.EducationLevel)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                query = query.Where(p => p.User.Location != null && p.User.Location.Contains(filter.Location));
            }

            var hasYearsFilter = filter.MinYearsExperience.HasValue || filter.MaxYearsExperience.HasValue;

            if (!hasYearsFilter)
            {
                if (!usePaging)
                {
                    var allItems = await query.ToListAsync();
                    return new PageResponse<ProfessionalProfile>
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

                return new PageResponse<ProfessionalProfile>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            // Years filter present — TotalYearsExperience is computed, not stored, so it
            // can't be translated to SQL. Materialize every DB-filtered match, then
            // filter and page in memory. Trade-off of Option A: on a large candidate
            // table this fetches more rows than a purely DB-side query would.
            var candidates = await query.ToListAsync();

            var filtered = candidates.Where(p =>
            {
                var totalYears = p.Experiences
                    .Where(e => !e.IsDeleted)
                    .Sum(e => ((e.IsCurrentJob ? DateTime.UtcNow : e.EndDate) - e.StartDate).Days / 365);

                return (!filter.MinYearsExperience.HasValue || totalYears >= filter.MinYearsExperience.Value)
                    && (!filter.MaxYearsExperience.HasValue || totalYears <= filter.MaxYearsExperience.Value);
            }).ToList();

            if (!usePaging)
            {
                return new PageResponse<ProfessionalProfile>
                {
                    Items = filtered,
                    TotalCount = filtered.Count,
                    PageNumber = 1,
                    PageSize = filtered.Count
                };
            }

            var pagedItems = filtered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PageResponse<ProfessionalProfile>
            {
                Items = pagedItems,
                TotalCount = filtered.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<TalentStatsDto> GetTalentStatsAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

            var baseQuery = context.ProfessionalProfiles.AsNoTracking().Where(p => !p.IsDeleted);

            var totalCandidates = await baseQuery.CountAsync();
            var newThisWeek = await baseQuery.CountAsync(p => p.DateCreated >= oneWeekAgo);

            var topSkills = await context.ProfessionalSkills
            .AsNoTracking()
            .Where(ps => !ps.IsDeleted && !ps.ProfessionalProfile.IsDeleted)
            .GroupBy(ps => ps.Skill.Name)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new TopSkillDto(g.Key, g.Count()))
            .ToListAsync();

            return new TalentStatsDto
            {
                TotalCandidates = totalCandidates,
                NewThisWeek = newThisWeek,
                TopSkills = topSkills
            };
        }
    }
}