using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IRecruiterProfileRepository
    {
        Task CreateAsync(RecruiterProfile recruiterProfile);

        Task<RecruiterProfile?> GetByIdAsync(Guid id);

        Task<RecruiterProfile?> GetByUserIdAsync(Guid userId);

        Task<PageResponse<RecruiterProfile>> GetByCompanyIdAsync(PageRequest request, bool usePaging, Guid companyId, RecruiterStatus? status);

        Task<List<RecruiterProfile>> GetCompanyAdminsAsync(Guid companyId);

        Task<Dictionary<Guid, int>> CountByCompanyIdsAsync(List<Guid> companyIds);

        void UpdateAsync(RecruiterProfile recruiterProfile);

        void Delete(RecruiterProfile recruiterProfile);
    }
}