using Application.Common.Dtos;
using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminPosts
    {
        public record GetAdminPostsQuery(
            string? Search,
            string? Status,
            DateTime? PostedFrom,
            DateTime? PostedTo,
            PageRequest PageRequest,
            bool UsePaging) : IRequest<Result<PageResponse<AdminPostListItem>>>;

        public class GetAdminPostsHandler(IPostRepository postRepository)
            : IRequestHandler<GetAdminPostsQuery, Result<PageResponse<AdminPostListItem>>>
        {
            public async Task<Result<PageResponse<AdminPostListItem>>> Handle(
                GetAdminPostsQuery request, CancellationToken cancellationToken)
            {
                var filter = new AdminPostFilter(request.Search, request.Status, request.PostedFrom, request.PostedTo);

                var posts = await postRepository.GetAdminListAsync(filter, request.PageRequest, request.UsePaging);

                var response = new PageResponse<AdminPostListItem>
                {
                    Items = posts.Items.Select(p => new AdminPostListItem(
                        p.Id,
                        p.Content,
                        p.User.FirstName + " " + p.User.LastName,
                        p.User.ProfilePictureUrl,
                        p.Visibility.ToString(),
                        ComputeStatus(p.IsDeleted, p.Reports.Any(r => r.ReportStatus == ReportStatus.Pending)),
                        p.Reports.Count(r => r.ReportStatus == ReportStatus.Pending),
                        p.Attachments.Count,
                        p.DateCreated)).ToList(),
                    TotalCount = posts.TotalCount,
                    PageNumber = posts.PageNumber,
                    PageSize = posts.PageSize
                };

                return Result<PageResponse<AdminPostListItem>>.Success(response, "Posts retrieved successfully");
            }

            private static string ComputeStatus(bool isDeleted, bool hasPendingReports) =>
                isDeleted ? "Removed" : hasPendingReports ? "Flagged" : "Active";
        }
    }

    public record AdminPostListItem(
        Guid PostId,
        string Content,
        string AuthorName,
        string? AuthorAvatarUrl,
        string Visibility,
        string Status,
        int PendingReportsCount,
        int AttachmentsCount,
        DateTime PostedDate);
}