using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Admin
{
    public class UpdateCompanyStatus
    {
        public record UpdateCompanyStatusCommand(
            Guid CompanyId,
            Guid ActingAdminId,
            CompanyStatus NewStatus,
            string Action,
            string? Reason) : IRequest<Result<object>>;

        public class UpdateCompanyStatusHandler(
            ICompanyRepository companyRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateCompanyStatusCommand, Result<object>>
        {
            public async Task<Result<object>> Handle(UpdateCompanyStatusCommand request, CancellationToken cancellationToken)
            {
                var company = await companyRepository.GetByIdAsync(request.CompanyId);

                if (company is null)
                    return Result<object>.Failure("Company not found");

                if (company.Status == request.NewStatus)
                    return Result<object>.Failure($"Company is already {request.NewStatus}");

                var previousStatus = company.Status;
                company.Status = request.NewStatus;

                if (request.NewStatus == CompanyStatus.Active && !company.IsVerified)
                {
                    company.IsVerified = true;
                    company.VerifiedAt = DateTime.UtcNow;
                }

                companyRepository.UpdateAsync(company);

                await auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = request.ActingAdminId,
                    Action = request.Action,
                    Description = string.IsNullOrWhiteSpace(request.Reason)
                        ? $"Changed {company.Name}'s status from {previousStatus} to {request.NewStatus}"
                        : $"Changed {company.Name}'s status from {previousStatus} to {request.NewStatus}: {request.Reason}",
                    CreatedBy = request.ActingAdminId.ToString()
                });

                await unitOfWork.SaveAsync();

                return Result<object>.Success(new { }, $"Company {request.Action.ToLower()}ed successfully");
            }
        }
    }
}