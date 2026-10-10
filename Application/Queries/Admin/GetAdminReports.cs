using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminReports
    {
        public record GetAdminReportsQuery(
            string? ContentType,
            ReportStatus? Status,
            ReportReason? Reason,
            string? Search,
            DateTime? CreatedFrom,
            DateTime? CreatedTo,
            PageRequest PageRequest,
            bool UsePaging) : IRequest<Result<PageResponse<AdminReportListItem>>>;

        public class GetAdminReportsHandler(IReportRepository reportRepository)
            : IRequestHandler<GetAdminReportsQuery, Result<PageResponse<AdminReportListItem>>>
        {
            public async Task<Result<PageResponse<AdminReportListItem>>> Handle(
                GetAdminReportsQuery request, CancellationToken cancellationToken)
            {
                var filter = new AdminReportFilter(
                    request.ContentType, request.Status, request.Reason,
                    request.Search, request.CreatedFrom, request.CreatedTo);

                var reports = await reportRepository.GetAdminListAsync(filter, request.PageRequest, request.UsePaging);

                var response = new PageResponse<AdminReportListItem>
                {
                    Items = reports.Items.Select(r => new AdminReportListItem(
                        r.Id,
                        r.ContentType,
                        r.Post != null ? TrimContent(r.Post.Content) : null,
                        r.Reporter.FirstName + " " + r.Reporter.LastName,
                        r.ReportedUser.FirstName + " " + r.ReportedUser.LastName,
                        r.Reason.ToString(),
                        r.ReportStatus.ToString(),
                        r.DateCreated)).ToList(),
                    TotalCount = reports.TotalCount,
                    PageNumber = reports.PageNumber,
                    PageSize = reports.PageSize
                };

                return Result<PageResponse<AdminReportListItem>>.Success(response, "Reports retrieved successfully");
            }

            private static string TrimContent(string content) =>
                content.Length > 80 ? content[..80] + "…" : content;
        }
    }

    public record AdminReportListItem(
        Guid ReportId,
        string ContentType,
        string? ContentPreview,
        string ReporterName,
        string ReportedUserName,
        string Reason,
        string Status,
        DateTime DateReported);
}