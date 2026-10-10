using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminCompanyDetails
    {
        public record GetAdminCompanyDetailsQuery(Guid CompanyId) : IRequest<Result<AdminCompanyDetailsResponse>>;

        public class GetAdminCompanyDetailsHandler(ICompanyRepository companyRepository)
            : IRequestHandler<GetAdminCompanyDetailsQuery, Result<AdminCompanyDetailsResponse>>
        {
            public async Task<Result<AdminCompanyDetailsResponse>> Handle(
                GetAdminCompanyDetailsQuery request, CancellationToken cancellationToken)
            {
                var company = await companyRepository.GetByIdWithDetailsAsync(request.CompanyId);

                if (company is null)
                    return Result<AdminCompanyDetailsResponse>.Failure("Company not found");

                var response = new AdminCompanyDetailsResponse(
                    company.Id,
                    company.Name,
                    company.LogoUrl,
                    company.Industry,
                    company.Description,
                    company.Website,
                    company.Email,
                    company.PhoneNumber,
                    company.Headquarters,
                    company.CompanySize,
                    company.FoundedYear,
                    company.Status.ToString(),
                    company.IsVerified,
                    company.DateCreated,
                    company.Jobs.Count,
                    company.Jobs.Sum(j => j.JobApplications.Count),
                    company.RecruiterProfiles
                        .Select(r => new AdminCompanyRecruiterItem(
                            r.Id,
                            r.User.FirstName + " " + r.User.LastName,
                            r.User.ProfilePictureUrl,
                            r.JobTitle,
                            r.IsCompanyAdmin,
                            r.Status.ToString()))
                        .ToList());

                return Result<AdminCompanyDetailsResponse>.Success(response, "Company details retrieved successfully");
            }
        }
    }

    public record AdminCompanyRecruiterItem(
        Guid RecruiterProfileId,
        string Name,
        string? AvatarUrl,
        string? JobTitle,
        bool IsCompanyAdmin,
        string Status);

    public record AdminCompanyDetailsResponse(
        Guid CompanyId,
        string Name,
        string? LogoUrl,
        string Industry,
        string Description,
        string? Website,
        string Email,
        string PhoneNumber,
        string? Headquarters,
        string CompanySize,
        int? FoundedYear,
        string Status,
        bool IsVerified,
        DateTime CreatedDate,
        int ActiveJobsCount,
        int TotalApplicationsCount,
        List<AdminCompanyRecruiterItem> Recruiters);
}