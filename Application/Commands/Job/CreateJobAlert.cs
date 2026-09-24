using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class CreateJobAlert
    {
        public record CreateJobAlertCommand(
            Guid ProfessionalProfileId,
            string? Keyword,
            string? Location,
            Guid? JobCategoryId,
            EmploymentType? EmploymentType,
            WorkPlaceType? WorkPlaceType,
            ExperienceLevel? ExperienceLevel,
            decimal? MinSalary,
            string CreatedBy
        ) : IRequest<Result<string>>;

        public class CreateJobAlertHandler(
            ISavedJobSearchRepository savedJobSearchRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateJobAlertCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(CreateJobAlertCommand request, CancellationToken cancellationToken)
            {
                var search = new SavedJobSearch
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,
                    Keyword = request.Keyword,
                    Location = request.Location,
                    DepartmentId = request.JobCategoryId,
                    EmploymentType = request.EmploymentType,
                    WorkPlaceType = request.WorkPlaceType,
                    ExperienceLevel = request.ExperienceLevel,
                    MinSalary = request.MinSalary,
                    EmailNotificationsEnabled = true,
                    CreatedBy = request.CreatedBy,
                    DateCreated = DateTime.UtcNow
                };

                await savedJobSearchRepository.AddAsync(search);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(search.Id.ToString(), "Job alert created successfully");
            }
        }
    }
}