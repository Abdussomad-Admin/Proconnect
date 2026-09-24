using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using static Application.Queries.GetCompanyManagementOverview;

namespace Application.Commands
{
    public class UploadCompanyCoverImage
    {
        public record UploadCompanyCoverImageCommand(
            Guid RequestingUserId,
            IFormFile File
        ) : IRequest<Result<string>>;

        public class UploadCompanyCoverImageHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<UploadCompanyCoverImageCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UploadCompanyCoverImageCommand request,
                CancellationToken cancellationToken)
            {
                var requestingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (requestingProfile is null || requestingProfile.CompanyId is null)
                {
                    return Result<string>.Failure("You are not linked to a company");
                }

                if (requestingProfile.Status != RecruiterStatus.Active)
                    return Result<string>.Failure("Your membership is pending");

                if (!requestingProfile.IsCompanyAdmin)
                {
                    return Result<string>.Failure("Only a company admin can update the company cover image");
                }

                var company = await companyRepository.GetByIdAsync(requestingProfile.CompanyId.Value);

                if (company is null)
                {
                    return Result<string>.Failure("Company not found");
                }

                if (request.File is null || request.File.Length == 0)
                {
                    return Result<string>.Failure("No file was uploaded");
                }

                if (!string.IsNullOrWhiteSpace(company.CoverImagePublicId))
                {
                    await fileStorage.DeleteAsync(company.CoverImagePublicId, cancellationToken);
                }

                var uploadResult = await fileStorage.UploadAsync(
                    request.File,
                    "proconnect/company-cover-images",
                    cancellationToken);

                company.CoverImageUrl = uploadResult.Url;
                company.CoverImagePublicId = uploadResult.PublicId;

                companyRepository.UpdateAsync(company);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingProfile.UserId,
                        Action = "UploadCompanyCoverImage",
                        CreatedBy = requestingProfile.UserId.ToString(),
                        Description = $"Admin updated company cover image: {company.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<string>.Success(uploadResult.Url, "Cover image updated successfully");
            }

            public record UploadCompanyCoverImageDto(IFormFile File);
        }
    }
}