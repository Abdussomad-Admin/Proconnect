namespace Application.Common.Dtos.Admin
{
    public record AdminPostFilter(
        string? Search,
        string? Status,
        DateTime? PostedFrom,
        DateTime? PostedTo);
}