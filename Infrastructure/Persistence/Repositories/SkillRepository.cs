using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SkillRepository(ProConnectDbContext context) : ISkillRepository
    {
        public async Task<List<Skill>> GetAllAsync()
        {
            return await context.Skills
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Skill?> GetByIdAsync(Guid id)
        {
            return await context.Skills
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task AddAsync(Skill skill)
        {
            await context.Skills.AddAsync(skill);
        }

        public async Task<List<Skill>> SearchByNameAsync(string query, int limit)
        {
            return await context.Skills
                .Where(s => !s.IsDeleted && s.Name.Contains(query))
                .OrderBy(s => s.Name)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Skill?> GetByNameAsync(string name)
        {
            return await context.Skills
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.Name.ToLower() == name.ToLower());
        }
    }
}