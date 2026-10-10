using Application.Common.Dtos;
using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminCompanies
    {
        public record GetAdminCompaniesQuery(
            string? Search,
            string? Industry,
            string? Location,
            CompanyStatus? Status,
            DateTime? CreatedFrom,
            DateTime? CreatedTo,
            PageRequest PageRequest,
            bool UsePaging) : IRequest<Result<PageResponse<AdminCompanyListItem>>>;

        public class GetAdminCompaniesHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository)
            : IRequestHandler<GetAdminCompaniesQuery, Result<PageResponse<AdminCompanyListItem>>>
        {
            public async Task<Result<PageResponse<AdminCompanyListItem>>> Handle(
                GetAdminCompaniesQuery request, CancellationToken cancellationToken)
            {
                var filter = new AdminCompanyFilter(
                    request.Search, request.Industry, request.Location,
                    request.Status, request.CreatedFrom, request.CreatedTo);

                var companies = await companyRepository.GetAdminListAsync(filter, request.PageRequest, request.UsePaging);

                var companyIds = companies.Items.Select(c => c.Id).ToList();
                var recruiterCounts = await recruiterProfileRepository.CountByCompanyIdsAsync(companyIds);

                var response = new PageResponse<AdminCompanyListItem>
                {
                    Items = companies.Items.Select(c => new AdminCompanyListItem(
                        c.Id,
                        c.Name,
                        c.LogoUrl,
                        c.Industry,
                        c.Headquarters,
                        c.Website,
                        c.Status.ToString(),
                        recruiterCounts.GetValueOrDefault(c.Id),
                        c.DateCreated)).ToList(),
                    TotalCount = companies.TotalCount,
                    PageNumber = companies.PageNumber,
                    PageSize = companies.PageSize
                };

                return Result<PageResponse<AdminCompanyListItem>>.Success(response, "Companies retrieved successfully");
            }
        }
    }

    public record AdminCompanyListItem(
        Guid CompanyId,
        string Name,
        string? LogoUrl,
        string Industry,
        string? Headquarters,
        string? Website,
        string Status,
        int RecruiterCount,
        DateTime CreatedDate);
}