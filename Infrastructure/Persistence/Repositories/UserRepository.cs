using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository(ProConnectDbContext proConnectDb) : IUserRepository
    {
        public async Task<int> CountAsync()
        {
            return await proConnectDb.Users.CountAsync(x => !x.IsDeleted);
        }

        public async Task CreateAsync(User user)
        {
            await proConnectDb.Users.AddAsync(user);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await proConnectDb.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName)
        {
            return await proConnectDb.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<User?> GetByIdWithAdminDetailsAsync(Guid id)
        {
            return await proConnectDb.Users
                .Include(x => x.RecruiterProfile).ThenInclude(r => r!.Company)
                .Include(x => x.RecruiterProfile).ThenInclude(r => r!.Jobs)
                .Include(x => x.RecruiterProfile).ThenInclude(r => r!.Events)
                .Include(x => x.ProfessionalProfile).ThenInclude(p => p!.JobApplications)
                .Include(x => x.ProfessionalProfile).ThenInclude(p => p!.SavedJobs)
                .Include(x => x.Posts)
                .Include(x => x.Following)
                .Include(x => x.Followers)
                .Include(x => x.ReportsReceived)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<PageResponse<User>> GetAdminListAsync(AdminUserFilter filter, PageRequest pageRequest, bool usePaging)
        {
            var query = proConnectDb.Users
                .Include(x => x.RecruiterProfile).ThenInclude(r => r!.Company)
                .Include(x => x.ProfessionalProfile)
                .Where(u => !u.IsDeleted && u.Role.ToLower() != "admin");

            if (string.Equals(filter.Type, "professional", StringComparison.OrdinalIgnoreCase))
                query = query.Where(u => u.ProfessionalProfile != null);
            else if (string.Equals(filter.Type, "recruiter", StringComparison.OrdinalIgnoreCase))
                query = query.Where(u => u.RecruiterProfile != null);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(u =>
                    u.FirstName.Contains(filter.Search) ||
                    u.LastName.Contains(filter.Search) ||
                    u.UserName.Contains(filter.Search) ||
                    u.Email.Contains(filter.Search));

            if (filter.Status.HasValue)
                query = query.Where(u => u.Status == filter.Status.Value);

            if (filter.RegisteredFrom.HasValue)
                query = query.Where(u => u.DateCreated >= filter.RegisteredFrom.Value);

            if (filter.RegisteredTo.HasValue)
                query = query.Where(u => u.DateCreated < filter.RegisteredTo.Value);

            query = query.OrderByDescending(u => u.DateCreated);

            if (usePaging)
            {
                var offset = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);

                return new PageResponse<User>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<User>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public async Task<PageResponse<User>> GetAllAsync(PageRequest pageRequest, bool usePaging)
        {
            var query = proConnectDb.Users
                .Include(x => x.RecruiterProfile)
                .Include(x => x.ProfessionalProfile)
                .AsQueryable();

            if (usePaging)
            {
                var offset = query
                    .Skip((pageRequest.PageNumber -1 ) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize);

                return new PageResponse<User>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<User>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.PasswordResetToken == token && !u.IsDeleted);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.UserName == userName && !u.IsDeleted);
        }

        public async Task<User?> GetByVerificationTokenAsync(string token)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.VerificationToken == token && !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> SearchAsync(string keyword)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .Where(u => u.FirstName.Contains(keyword) || u.LastName.Contains(keyword) || u.UserName.Contains(keyword) || u.Email.Contains(keyword) && !u.IsDeleted)

                .ToListAsync();
        }

        public void UpdateAsync(User user)
        {
            proConnectDb.Users.Update(user);
        }
    }
}
