using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ICompanyRepository
    {
        Task CreateAsync(Company company);

        Task<Company?> GetByIdAsync(Guid id);

        Task<Company?> GetByIdWithDetailsAsync(Guid id);

        Task<bool> ExistsByNameAsync(string name);

        Task<Company?> GetByInvitationCodeAsync(string invitationCode);

        Task<PageResponse<Company>> GetAdminListAsync(AdminCompanyFilter filter, PageRequest pageRequest, bool usePaging);

        void UpdateAsync(Company company);

        void Delete(Company company);
    }
}