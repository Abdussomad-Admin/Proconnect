using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class UpdateDepartment
    {
        public record UpdateDepartmentCommand(
            Guid RequestingUserId,
            Guid DepartmentId,
            string Name,
            string? Description
        ) : IRequest<Result<Guid>>;

        public class UpdateDepartmentHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<UpdateDepartmentCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                UpdateDepartmentCommand request,
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
                    return Result<Guid>.Failure("Only a company admin can update departments");
                }

                var department = await departmentRepository.GetByIdAsync(request.DepartmentId);

                if (department is null || department.CompanyId != requestingProfile.CompanyId)
                {
                    return Result<Guid>.Failure("Department not found");
                }

                var name = request.Name.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    return Result<Guid>.Failure("Department name is required");
                }

                if (!string.Equals(department.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = await departmentRepository.ExistsByNameAsync(requestingProfile.CompanyId.Value, name);

                    if (exists)
                    {
                        return Result<Guid>.Failure("A department with this name already exists");
                    }
                }

                department.Name = name;
                department.Description = request.Description?.Trim();
                department.DateModified = DateTime.UtcNow;

                departmentRepository.UpdateAsync(department);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingProfile.UserId,
                        Action = "UpdateDepartment",
                        CreatedBy = requestingProfile.UserId.ToString(),
                        Description = $"Admin updated department: {department.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<Guid>.Success(department.Id, "Department updated successfully");
            }
        }
    }
}