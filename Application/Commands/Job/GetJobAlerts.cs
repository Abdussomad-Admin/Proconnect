using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class GetJobAlerts
    {
        public record GetJobAlertsQuery(Guid ProfessionalProfileId) : IRequest<Result<List<GetJobAlertsResponse>>>;

        public class GetJobAlertsHandler(
            ISavedJobSearchRepository savedJobSearchRepository)
            : IRequestHandler<GetJobAlertsQuery, Result<List<GetJobAlertsResponse>>>
        {
            public async Task<Result<List<GetJobAlertsResponse>>> Handle(GetJobAlertsQuery request, CancellationToken cancellationToken)
            {
                var searches = await savedJobSearchRepository.GetByProfessionalProfileIdAsync(request.ProfessionalProfileId);

                var response = searches.Select(s => new GetJobAlertsResponse(
                    s.Id, s.Keyword, s.Location, s.DepartmentId, s.Department?.Name,
                    s.EmploymentType, s.WorkPlaceType, s.ExperienceLevel, s.MinSalary,
                    s.EmailNotificationsEnabled, s.DateCreated)).ToList();

                return Result<List<GetJobAlertsResponse>>.Success(response, "Job alerts retrieved successfully");
            }
        }
    }

    public record GetJobAlertsResponse(
        Guid Id, string? Keyword, string? Location, Guid? JobCategoryId, string? CategoryName,
        EmploymentType? EmploymentType, WorkPlaceType? WorkPlaceType, ExperienceLevel? ExperienceLevel,
        decimal? MinSalary, bool EmailNotificationsEnabled, DateTime DateCreated);
}