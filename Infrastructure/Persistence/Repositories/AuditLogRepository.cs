using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository(ProConnectDbContext context) : IAuditLogRepository
    {
        public async Task AddAsync(AuditLog auditLog)
        {
            await context.AuditLogs.AddAsync(auditLog);
        }


        public async Task<AuditLog?> GetByIdAsync(Guid id)
        {
            return await context.AuditLogs
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<PageResponse<AuditLog>> GetAdminListAsync(AdminAuditLogFilter filter, PageRequest pageRequest, bool usePaging)
        {
            var query = context.AuditLogs
                .AsNoTracking()
                .Include(a => a.User)
                .Where(a => !a.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(a =>
                    a.Description.Contains(filter.Search) ||
                    a.User.UserName.Contains(filter.Search) ||
                    a.Action.Contains(filter.Search));

            if (!string.IsNullOrWhiteSpace(filter.Action))
                query = query.Where(a => a.Action == filter.Action);

            if (filter.UserId.HasValue)
                query = query.Where(a => a.UserId == filter.UserId.Value);

            if (filter.From.HasValue)
                query = query.Where(a => a.Timestamp >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(a => a.Timestamp < filter.To.Value);

            query = query.OrderByDescending(a => a.Timestamp);

            if (usePaging)
            {
                var offset = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);

                return new PageResponse<AuditLog>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<AuditLog>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }
    }
}