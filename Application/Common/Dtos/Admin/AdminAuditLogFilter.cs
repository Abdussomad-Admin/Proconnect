namespace Application.Common.Dtos
{
    public record AdminAuditLogFilter(
        string? Search,
        string? Action,
        Guid? UserId,
        DateTime? From,
        DateTime? To);
}