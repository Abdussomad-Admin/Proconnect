using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IDepartmentRepository
    {
        Task CreateAsync(Department department);
        Task<Department?> GetByIdAsync(Guid id);
        Task<bool> ExistsByNameAsync(Guid companyId, string name);
        Task<List<Department>> GetByCompanyIdAsync(Guid companyId);
        void UpdateAsync(Department department);
        void Delete(Department department);
    }
}