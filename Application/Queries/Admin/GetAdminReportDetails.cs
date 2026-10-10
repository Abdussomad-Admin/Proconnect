using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminReportDetails
    {
        public record GetAdminReportDetailsQuery(Guid ReportId) : IRequest<Result<AdminReportDetailsResponse>>;

        public class GetAdminReportDetailsHandler(IReportRepository reportRepository)
            : IRequestHandler<GetAdminReportDetailsQuery, Result<AdminReportDetailsResponse>>
        {
            public async Task<Result<AdminReportDetailsResponse>> Handle(
                GetAdminReportDetailsQuery request, CancellationToken cancellationToken)
            {
                var report = await reportRepository.GetByIdAsync(request.ReportId);

                if (report is null)
                    return Result<AdminReportDetailsResponse>.Failure("Report not found");

                var response = new AdminReportDetailsResponse(
                    report.Id,
                    report.ContentType,
                    report.Reason.ToString(),
                    report.ReportStatus.ToString(),
                    report.DateCreated,
                    new AdminReportPersonSummary(report.ReporterId, report.Reporter.FirstName + " " + report.Reporter.LastName, report.Reporter.ProfilePictureUrl),
                    new AdminReportPersonSummary(report.ReportedUserId, report.ReportedUser.FirstName + " " + report.ReportedUser.LastName, report.ReportedUser.ProfilePictureUrl),
                    report.Post == null ? null : new AdminReportedPost(
                        report.Post.Id,
                        report.Post.Content,
                        report.Post.User.FirstName + " " + report.Post.User.LastName,
                        report.Post.DateCreated,
                        report.Post.Attachments.Select(a => a.FileUrl).ToList()));

                return Result<AdminReportDetailsResponse>.Success(response, "Report details retrieved successfully");
            }
        }
    }

    public record AdminReportPersonSummary(Guid UserId, string Name, string? AvatarUrl);

    public record AdminReportedPost(Guid PostId, string Content, string AuthorName, DateTime PostedAt, List<string> AttachmentUrls);

    public record AdminReportDetailsResponse(
        Guid ReportId,
        string ContentType,
        string Reason,
        string Status,
        DateTime DateReported,
        AdminReportPersonSummary Reporter,
        AdminReportPersonSummary ReportedUser,
        AdminReportedPost? Post);
}