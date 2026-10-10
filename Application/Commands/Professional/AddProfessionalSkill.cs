using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Professional
{
    public class AddProfessionalSkill
    {
        public record AddProfessionalSkillCommand(
            Guid ProfessionalProfileId,
            Guid SkillId,
            string Level,
            int YearsOfExperience
        ) : IRequest<Result<string>>;

        public class AddProfessionalSkillHandler(
            IProfessionalSkillRepository professionalSkillRepository,
            IProfessionalProfileRepository professionalProfileRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<
                AddProfessionalSkillCommand,
                Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddProfessionalSkillCommand request,
                CancellationToken cancellationToken)
            {
                var exists = await professionalSkillRepository.ExistsAsync(
                    request.ProfessionalProfileId,
                    request.SkillId);


                if (exists)
                {
                    return Result<string>.Failure(
                        "Skill already added");
                }

                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile == null)
                {
                    return Result<string>.Failure(
                        "Professional profile not found");
                }


                var professionalSkill = new ProfessionalSkill
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,

                    SkillId = request.SkillId,

                    Level = request.Level,

                    CreatedBy = profile.UserId.ToString(),

                    YearsOfExperience = request.YearsOfExperience,

                    DateCreated = DateTime.UtcNow
                };


                await professionalSkillRepository.CreateAsync(professionalSkill);

                await unitOfWork.SaveAsync();


                return Result<string>.Success(
                    "Skill added successfully",
                    "created");
            }
        }
    }
}