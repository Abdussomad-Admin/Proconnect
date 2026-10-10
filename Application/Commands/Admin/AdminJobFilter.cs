using Domain.Enums;

namespace Application.Common.Dtos.Admin
{
    public record AdminJobFilter(
        string? Search,
        JobPostingStatus? Status,
        string? Industry,
        string? Location,
        DateTime? PostedFrom,
        DateTime? PostedTo);
}