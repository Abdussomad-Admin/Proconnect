using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminActivityLogs
    {
        public record GetAdminActivityLogsQuery(
            string? Search,
            string? Action,
            Guid? UserId,
            DateTime? From,
            DateTime? To,
            PageRequest PageRequest,
            bool UsePaging) : IRequest<Result<PageResponse<AdminActivityLogItem>>>;

        public class GetAdminActivityLogsHandler(IAuditLogRepository auditLogRepository)
            : IRequestHandler<GetAdminActivityLogsQuery, Result<PageResponse<AdminActivityLogItem>>>
        {
            public async Task<Result<PageResponse<AdminActivityLogItem>>> Handle(
                GetAdminActivityLogsQuery request, CancellationToken cancellationToken)
            {
                var filter = new AdminAuditLogFilter(request.Search, request.Action, request.UserId, request.From, request.To);

                var logs = await auditLogRepository.GetAdminListAsync(filter, request.PageRequest, request.UsePaging);

                var response = new PageResponse<AdminActivityLogItem>
                {
                    Items = logs.Items.Select(a => new AdminActivityLogItem(
                        a.Id,
                        a.UserId,
                        a.User.FirstName + " " + a.User.LastName,
                        a.User.ProfilePictureUrl,
                        a.Action,
                        a.Description,
                        a.Timestamp)).ToList(),
                    TotalCount = logs.TotalCount,
                    PageNumber = logs.PageNumber,
                    PageSize = logs.PageSize
                };

                return Result<PageResponse<AdminActivityLogItem>>.Success(response, "Activity logs retrieved successfully");
            }
        }
    }

    public record AdminActivityLogItem(
        Guid LogId,
        Guid UserId,
        string UserName,
        string? UserAvatarUrl,
        string Action,
        string Description,
        DateTime Timestamp);
}