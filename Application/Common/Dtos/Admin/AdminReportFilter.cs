using Domain.Enums;

namespace Application.Common.Dtos
{
    public record AdminReportFilter(
        string? ContentType,
        ReportStatus? Status,
        ReportReason? Reason,
        string? Search,
        DateTime? CreatedFrom,
        DateTime? CreatedTo);
}