using Application.Common.Dtos;
using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<AuditLog?> GetByIdAsync(Guid id);
        Task<PageResponse<AuditLog>> GetAdminListAsync(AdminAuditLogFilter filter, PageRequest pageRequest, bool usePaging);
    }
}
