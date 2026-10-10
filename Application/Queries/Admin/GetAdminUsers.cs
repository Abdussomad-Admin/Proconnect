using Application.Common.Dtos;
using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminUsers
    {
        public record GetAdminUsersQuery(
            string? Type,
            string? Search,
            UserStatus? Status,
            DateTime? RegisteredFrom,
            DateTime? RegisteredTo,
            PageRequest PageRequest,
            bool UsePaging) : IRequest<Result<PageResponse<AdminUserListItem>>>;

        public class GetAdminUsersHandler(IUserRepository userRepository)
            : IRequestHandler<GetAdminUsersQuery, Result<PageResponse<AdminUserListItem>>>
        {
            public async Task<Result<PageResponse<AdminUserListItem>>> Handle(
                GetAdminUsersQuery request, CancellationToken cancellationToken)
            {
                var filter = new AdminUserFilter(
                    request.Type, request.Search, request.Status,
                    request.RegisteredFrom, request.RegisteredTo);

                var users = await userRepository.GetAdminListAsync(filter, request.PageRequest, request.UsePaging);

                var response = new PageResponse<AdminUserListItem>
                {
                    Items = users.Items.Select(u => new AdminUserListItem(
                        u.Id,
                        u.FirstName + " " + u.LastName,
                        u.UserName,
                        u.Email,
                        u.ProfilePictureUrl,
                        u.ProfessionalProfile != null ? "Professional"
                            : u.RecruiterProfile != null ? "Recruiter" : "User",
                        u.Status.ToString(),
                        u.DateCreated)).ToList(),
                    TotalCount = users.TotalCount,
                    PageNumber = users.PageNumber,
                    PageSize = users.PageSize
                };

                return Result<PageResponse<AdminUserListItem>>.Success(response, "Users retrieved successfully");
            }
        }
    }

    public record AdminUserListItem(
        Guid UserId,
        string Name,
        string UserName,
        string Email,
        string? AvatarUrl,
        string Type,
        string Status,
        DateTime RegisteredDate);
}