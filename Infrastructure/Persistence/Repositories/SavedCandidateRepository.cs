using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SavedCandidateRepository(ProConnectDbContext context) : ISavedCandidateRepository
    {
        public async Task<bool> ExistsAsync(Guid recruiterProfileId, Guid professionalProfileId)
        {
            return await context.SavedCandidates
                .AnyAsync(sc => sc.RecruiterProfileId == recruiterProfileId
                    && sc.ProfessionalProfileId == professionalProfileId
                    && !sc.IsDeleted);
        }

        public async Task<SavedCandidate?> GetByRecruiterAndProfileAsync(Guid recruiterProfileId, Guid professionalProfileId)
        {
            return await context.SavedCandidates
                .FirstOrDefaultAsync(sc => sc.RecruiterProfileId == recruiterProfileId
                    && sc.ProfessionalProfileId == professionalProfileId
                    && !sc.IsDeleted);
        }

        public async Task AddAsync(SavedCandidate savedCandidate)
        {
            await context.SavedCandidates.AddAsync(savedCandidate);
        }

        public void Delete(SavedCandidate savedCandidate)
        {
            context.SavedCandidates.Remove(savedCandidate);
        }

        public async Task<PageResponse<SavedCandidate>> GetByRecruiterIdAsync(
            Guid recruiterProfileId,
            PageRequest request,
            bool usePaging)
        {
            var query = context.SavedCandidates
            .AsNoTracking()
            .Include(sc => sc.ProfessionalProfile)
                .ThenInclude(p => p.User)
            .Include(sc => sc.ProfessionalProfile)
                .ThenInclude(p => p.Experiences)
            .Include(sc => sc.ProfessionalProfile)
                .ThenInclude(p => p.Educations)
            .Include(sc => sc.ProfessionalProfile)
                .ThenInclude(p => p.ProfessionalSkills)
                    .ThenInclude(ps => ps.Skill)
            .Where(sc => sc.RecruiterProfileId == recruiterProfileId && !sc.IsDeleted)
            .OrderByDescending(sc => sc.DateCreated);

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();
                return new PageResponse<SavedCandidate>
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

            return new PageResponse<SavedCandidate>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}