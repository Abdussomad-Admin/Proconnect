using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class DepartmentRepository(ProConnectDbContext proConnectDb) : IDepartmentRepository
    {
        public async Task CreateAsync(Department department)
        {
            await proConnectDb.Departments.AddAsync(department);
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Departments
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<bool> ExistsByNameAsync(Guid companyId, string name)
        {
            return await proConnectDb.Departments
                .AnyAsync(d => d.CompanyId == companyId && !d.IsDeleted && d.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Department>> GetByCompanyIdAsync(Guid companyId)
        {
            return await proConnectDb.Departments
                .Where(d => d.CompanyId == companyId && !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public void UpdateAsync(Department department)
        {
            proConnectDb.Departments.Update(department);
        }

        public void Delete(Department department)
        {
            proConnectDb.Departments.Remove(department);
        }
    }
}