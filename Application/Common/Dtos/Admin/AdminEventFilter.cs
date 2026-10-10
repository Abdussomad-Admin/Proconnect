using Domain.Enums;

namespace Application.Common.Dtos
{
    public record AdminEventFilter(
        string? Search,
        EventStatus? Status,
        string? Location,
        DateTime? StartFrom,
        DateTime? StartTo);
}