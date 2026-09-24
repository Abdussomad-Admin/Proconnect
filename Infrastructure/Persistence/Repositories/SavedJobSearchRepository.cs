using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SavedJobSearchRepository(ProConnectDbContext context) : ISavedJobSearchRepository
    {
        public async Task AddAsync(SavedJobSearch search)
        {
            await context.SavedJobSearches.AddAsync(search);
        }

        public async Task<SavedJobSearch?> GetByIdAsync(Guid id)
        {
            return await context.SavedJobSearches
                .Include(s => s.ProfessionalProfile)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<List<SavedJobSearch>> GetByProfessionalProfileIdAsync(Guid professionalProfileId)
        {
            return await context.SavedJobSearches
                .AsNoTracking()
                .Include(s => s.Department)
                .Where(s => s.ProfessionalProfileId == professionalProfileId && !s.IsDeleted)
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync();
        }

        public async Task<List<SavedJobSearch>> GetAllActiveAsync()
        {
            return await context.SavedJobSearches
                .Include(s => s.ProfessionalProfile)
                    .ThenInclude(p => p.User)
                .Where(s => !s.IsDeleted && s.EmailNotificationsEnabled)
                .ToListAsync();
        }

        public void Delete(SavedJobSearch search)
        {
            context.SavedJobSearches.Remove(search);
        }

        public void Update(SavedJobSearch search)
        {
            context.SavedJobSearches.Update(search);
        }
    }
}