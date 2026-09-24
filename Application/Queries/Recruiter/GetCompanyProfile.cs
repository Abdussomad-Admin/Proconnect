using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;
using System.Text.Json;

namespace Application.Queries.Recruiter
{
    public class GetCompanyProfile
    {
        public record CompanyLocationDto(
            string City,
            string? Address,
            bool IsHeadquarters
        );

        public record CompanyProfileResponse(
            Guid Id,
            string Name,
            string Industry,
            string Description,
            string? Website,
            string Email,
            string PhoneNumber,
            string? LogoUrl,
            string? CoverImageUrl,
            string CompanySize,
            string CompanyType,
            int? FoundedYear,
            bool IsVerified,
            DateTime? VerifiedAt,
            string? LinkedInUrl,
            string? TwitterUrl,
            string? FacebookUrl,
            string? InstagramUrl,
            List<CompanyLocationDto> Locations,
            int TeamMemberCount,
            int OpenPositionCount
        );

        public record GetCompanyProfileQuery(Guid CompanyId) : IRequest<Result<CompanyProfileResponse>>;

        public class GetCompanyProfileHandler(
            ICompanyRepository companyRepository)
            : IRequestHandler<GetCompanyProfileQuery, Result<CompanyProfileResponse>>
        {
            public async Task<Result<CompanyProfileResponse>> Handle(
                GetCompanyProfileQuery request,
                CancellationToken cancellationToken)
            {
                var company = await companyRepository.GetByIdWithDetailsAsync(request.CompanyId);

                if (company is null)
                {
                    return Result<CompanyProfileResponse>.Failure("Company not found");
                }

                var locations = string.IsNullOrWhiteSpace(company.Locations)
                    ? new List<CompanyLocationDto>()
                    : JsonSerializer.Deserialize<List<CompanyLocationDto>>(company.Locations) ?? new List<CompanyLocationDto>();

                var result = new CompanyProfileResponse(
                    company.Id,
                    company.Name,
                    company.Industry,
                    company.Description,
                    company.Website,
                    company.Email,
                    company.PhoneNumber,
                    company.LogoUrl,
                    company.CoverImageUrl,
                    company.CompanySize,
                    company.CompanyType,
                    company.FoundedYear,
                    company.IsVerified,
                    company.VerifiedAt,
                    company.LinkedInUrl,
                    company.TwitterUrl,
                    company.FacebookUrl,
                    company.InstagramUrl,
                    locations,
                    company.RecruiterProfiles.Count,
                    company.Jobs.Count);

                return Result<CompanyProfileResponse>.Success(result, "Company profile retrieved");
            }
        }
    }
}