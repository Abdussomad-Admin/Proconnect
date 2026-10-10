using Domain.Enums;

namespace Application.Common.Dtos.Admin
{
    public record AdminUserFilter(
        string? Type,
        string? Search,
        UserStatus? Status,
        DateTime? RegisteredFrom,
        DateTime? RegisteredTo);
}
