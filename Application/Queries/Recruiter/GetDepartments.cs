using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries
{
    public class GetDepartments
    {
        public record DepartmentResponse(
            Guid Id,
            string Name,
            string? Description,
            int MemberCount,
            DateTime DateCreated
        );

        public record GetDepartmentsQuery(Guid RequestingUserId) : IRequest<Result<List<DepartmentResponse>>>;

        public class GetDepartmentsHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IDepartmentRepository departmentRepository)
            : IRequestHandler<GetDepartmentsQuery, Result<List<DepartmentResponse>>>
        {
            public async Task<Result<List<DepartmentResponse>>> Handle(
                GetDepartmentsQuery request,
                CancellationToken cancellationToken)
            {
                var requestingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (requestingProfile is null || requestingProfile.CompanyId is null)
                {
                    return Result<List<DepartmentResponse>>.Failure("You are not linked to a company");
                }

                var departments = await departmentRepository.GetByCompanyIdAsync(requestingProfile.CompanyId.Value);

                var teamPage = await recruiterProfileRepository.GetByCompanyIdAsync(
                    new PageRequest { PageNumber = 1, PageSize = 1 },
                    usePaging: false,
                    requestingProfile.CompanyId.Value,
                    status: null);

                var result = departments
                    .Select(d => new DepartmentResponse(
                        d.Id,
                        d.Name,
                        d.Description,
                        teamPage.Items.Count(m => string.Equals(m.Department, d.Name, StringComparison.OrdinalIgnoreCase)),
                        d.DateCreated))
                    .ToList();

                return Result<List<DepartmentResponse>>.Success(result, "Departments retrieved");
            }
        }
    }
}