using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CompanyRepository(ProConnectDbContext proConnectDb) : ICompanyRepository
    {
        public async Task CreateAsync(Company company)
        {
            await proConnectDb.Companies.AddAsync(company);
        }

        public async Task<Company?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Companies
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Company?> GetByIdWithDetailsAsync(Guid id)
        {
            return await proConnectDb.Companies
                .Include(c => c.RecruiterProfiles.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.User)
                .Include(c => c.Jobs.Where(j => j.IsActive))
                    .ThenInclude(j => j.JobApplications)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await proConnectDb.Companies
                .AnyAsync(c => c.Name.ToLower() == name.ToLower() && !c.IsDeleted);
        }

        public async Task<Company?> GetByInvitationCodeAsync(string invitationCode)
        {
            return await proConnectDb.Companies
                .FirstOrDefaultAsync(c => c.InvitationCode == invitationCode && !c.IsDeleted);
        }

        public async Task<PageResponse<Company>> GetAdminListAsync(AdminCompanyFilter filter, PageRequest pageRequest, bool usePaging)
        {
            var query = proConnectDb.Companies
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(c =>
                    c.Name.Contains(filter.Search) ||
                    c.Email.Contains(filter.Search) ||
                    (c.Website != null && c.Website.Contains(filter.Search)));

            if (!string.IsNullOrWhiteSpace(filter.Industry))
                query = query.Where(c => c.Industry == filter.Industry);

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(c => c.Headquarters != null && c.Headquarters.Contains(filter.Location));

            if (filter.Status.HasValue)
                query = query.Where(c => c.Status == filter.Status.Value);

            if (filter.CreatedFrom.HasValue)
                query = query.Where(c => c.DateCreated >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                query = query.Where(c => c.DateCreated < filter.CreatedTo.Value);

            query = query.OrderByDescending(c => c.DateCreated);

            if (usePaging)
            {
                var offset = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);

                return new PageResponse<Company>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<Company>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public void UpdateAsync(Company company)
        {
            proConnectDb.Companies.Update(company);
        }

        public void Delete(Company company)
        {
            proConnectDb.Companies.Remove(company);
        }
    }
}