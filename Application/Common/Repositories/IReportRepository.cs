using Application.Common.Dtos;
using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<Report?> GetByIdAsync(Guid id);
        Task<PageResponse<Report>> GetAdminListAsync(AdminReportFilter filter, PageRequest pageRequest, bool usePaging);
        Task<int> CountPendingByPostIdAsync(Guid postId);
        void Update(Report report);
    }
}