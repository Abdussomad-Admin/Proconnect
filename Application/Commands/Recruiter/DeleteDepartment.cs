using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class DeleteDepartment
    {
        public record DeleteDepartmentCommand(
            Guid RequestingUserId,
            Guid DepartmentId
        ) : IRequest<Result<Guid>>;

        public class DeleteDepartmentHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<DeleteDepartmentCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                DeleteDepartmentCommand request,
                CancellationToken cancellationToken)
            {
                var requestingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (requestingProfile is null || requestingProfile.CompanyId is null)
                {
                    return Result<Guid>.Failure("You are not linked to a company");
                }

                if (!requestingProfile.IsCompanyAdmin)
                {
                    return Result<Guid>.Failure("Only a company admin can delete departments");
                }

                var department = await departmentRepository.GetByIdAsync(request.DepartmentId);

                if (department is null || department.CompanyId != requestingProfile.CompanyId)
                {
                    return Result<Guid>.Failure("Department not found");
                }

                department.IsDeleted = true;
                department.DateModified = DateTime.UtcNow;

                departmentRepository.UpdateAsync(department);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingProfile.UserId,
                        Action = "DeleteDepartment",
                        CreatedBy = requestingProfile.UserId.ToString(),
                        Description = $"Admin deleted department: {department.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<Guid>.Success(department.Id, "Department deleted");
            }
        }
    }
}