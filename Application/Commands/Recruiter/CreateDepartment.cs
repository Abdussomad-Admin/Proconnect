using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Recruiter
{
    public class CreateDepartment
    {
        public record CreateDepartmentCommand(
            Guid RequestingUserId,
            string Name,
            string? Description
        ) : IRequest<Result<Guid>>;

        public class CreateDepartmentHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<CreateDepartmentCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                CreateDepartmentCommand request,
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
                    return Result<Guid>.Failure("Only a company admin can create departments");
                }

                var name = request.Name.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    return Result<Guid>.Failure("Department name is required");
                }

                var exists = await departmentRepository.ExistsByNameAsync(requestingProfile.CompanyId.Value, name);

                if (exists)
                {
                    return Result<Guid>.Failure("A department with this name already exists");
                }

                var department = new Department
                {
                    CompanyId = requestingProfile.CompanyId.Value,
                    Name = name,
                    Description = request.Description?.Trim(),
                    CreatedBy = requestingProfile.UserId.ToString(),
                    DateCreated = DateTime.UtcNow
                };

                await departmentRepository.CreateAsync(department);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingProfile.UserId,
                        Action = "CreateDepartment",
                        CreatedBy = requestingProfile.UserId.ToString(),
                        Description = $"Admin created department: {department.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<Guid>.Success(department.Id, "Department created successfully");
            }
        }
    }
}