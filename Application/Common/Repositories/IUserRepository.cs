using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);

        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUserNameAsync(string userName);

        Task<bool> ExistsByEmailAsync(string email);

        Task<bool> ExistsByUserNameAsync(string userName);

        Task<User?> GetByVerificationTokenAsync(string token);

        Task<User?> GetByPasswordResetTokenAsync(string token);

        Task<User?> GetByIdWithAdminDetailsAsync(Guid id);

        Task<User?> GetByRefreshTokenAsync(string refreshToken);

        Task<IEnumerable<User>> SearchAsync(string keyword);

        Task<PageResponse<User>> GetAdminListAsync(AdminUserFilter filter, PageRequest pageRequest, bool usePaging);

        Task<PageResponse<User>> GetAllAsync(PageRequest pageRequest, bool usePaging);

        Task<int> CountAsync();

        void UpdateAsync(User user);

    }
}