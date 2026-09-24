using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ISkillRepository
    {
        Task<List<Skill>> GetAllAsync();
        Task<Skill?> GetByIdAsync(Guid id);
        Task AddAsync(Skill skill);

        Task<List<Skill>> SearchByNameAsync(string query, int limit);

        Task<Skill?> GetByNameAsync(string name);
    }
}