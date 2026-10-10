using Domain.Enums;

namespace Application.Common.Dtos.Admin
{
    public record AdminCompanyFilter(
        string? Search,
        string? Industry,
        string? Location,
        CompanyStatus? Status,
        DateTime? CreatedFrom,
        DateTime? CreatedTo);
}