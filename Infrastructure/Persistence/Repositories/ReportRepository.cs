using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ReportRepository(ProConnectDbContext context) : IReportRepository
    {
        public async Task AddAsync(Report report)
        {
            await context.Reports.AddAsync(report);
        }

        public async Task<Report?> GetByIdAsync(Guid id)
        {
            return await context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.ReportedUser)
                .Include(r => r.Post).ThenInclude(p => p!.User)
                .Include(r => r.Post).ThenInclude(p => p!.Attachments.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<PageResponse<Report>> GetAdminListAsync(AdminReportFilter filter, PageRequest pageRequest, bool usePaging)
        {
            var query = context.Reports
                .AsNoTracking()
                .Include(r => r.Reporter)
                .Include(r => r.ReportedUser)
                .Include(r => r.Post)
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.ContentType))
                query = query.Where(r => r.ContentType == filter.ContentType);

            if (filter.Status.HasValue)
                query = query.Where(r => r.ReportStatus == filter.Status.Value);

            if (filter.Reason.HasValue)
                query = query.Where(r => r.Reason == filter.Reason.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(r =>
                    r.Reporter.UserName.Contains(filter.Search) ||
                    r.ReportedUser.UserName.Contains(filter.Search));

            if (filter.CreatedFrom.HasValue)
                query = query.Where(r => r.DateCreated >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                query = query.Where(r => r.DateCreated < filter.CreatedTo.Value);

            query = query.OrderByDescending(r => r.DateCreated);

            if (usePaging)
            {
                var offset = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);

                return new PageResponse<Report>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<Report>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public async Task<int> CountPendingByPostIdAsync(Guid postId)
        {
            return await context.Reports
                .AsNoTracking()
                .CountAsync(r => r.PostId == postId && r.ReportStatus == ReportStatus.Pending && !r.IsDeleted);
        }

        public void Update(Report report)
        {
            context.Reports.Update(report);
        }
    }
}