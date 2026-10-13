using Application.Common.Repositories;
using Application.Common.Dtos;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Professional
{
    public class AddExperience
    {
        public record AddExperienceCommand(
            Guid ProfessionalProfileId,
            string CompanyName,
            string JobTitle,
            EmploymentType? EmploymentType,
            string? Location,
            DateTime StartDate,
            DateTime? EndDate,
            bool IsCurrentJob,
            string? Description,
            string? CreatedBy
            ) : IRequest<Result<AddExperienceResponse>>;

        public class AddExperienceHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<AddExperienceCommand, Result<AddExperienceResponse>>
        {
            public async Task<Result<AddExperienceResponse>> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.CompanyName) ||
                    string.IsNullOrWhiteSpace(request.JobTitle))
                {
                    return Result<AddExperienceResponse>.Failure("Company and job title are required");
                }

                if (!request.IsCurrentJob && request.EndDate is null)
                {
                    return Result<AddExperienceResponse>.Failure("End date is required unless this is your current job");
                }

                var endDate = request.IsCurrentJob ? request.StartDate : request.EndDate!.Value;

                if (endDate < request.StartDate)
                {
                    return Result<AddExperienceResponse>.Failure("End date cannot be before the start date");
                }

                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile is null)
                    return Result<AddExperienceResponse>.Failure("Professional profile not found");

                var experience = new Experience
                {
                    ProfessionalProfileId = profile.Id,
                    CompanyName = request.CompanyName.Trim(),
                    JobTitle = request.JobTitle.Trim(),
                    EmploymentType = request.EmploymentType ?? default(EmploymentType),
                    Location = request.Location ?? string.Empty,
                    StartDate = request.StartDate,
                    EndDate = endDate,
                    IsCurrentJob = request.IsCurrentJob,
                    Description = request.Description ?? string.Empty,
                    CreatedBy = request.CreatedBy ?? string.Empty
                };

                await experienceRepository.CreateAsync(experience);

                await unitOfWork.SaveAsync();

                return Result<AddExperienceResponse>.Success(
                    new AddExperienceResponse(experience.Id, experience.CompanyName, experience.JobTitle),
                    "Experience added successfully");
            }
        }
    }

    public record AddExperienceResponse(Guid Id, string CompanyName, string JobTitle);
}