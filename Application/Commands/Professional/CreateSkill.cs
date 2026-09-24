using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Professional
{
    public class CreateSkill
    {
        public record CreateSkillCommand(string Name, string CreatedBy) : IRequest<Result<SkillCreatedResponse>>;

        public record SkillCreatedResponse(Guid Id, string Name);

        public class CreateSkillHandler(
            ISkillRepository skillRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateSkillCommand, Result<SkillCreatedResponse>>
        {
            public async Task<Result<SkillCreatedResponse>> Handle(
                CreateSkillCommand request,
                CancellationToken cancellationToken)
            {
                var name = request.Name.Trim();

                if (string.IsNullOrWhiteSpace(name))
                    return Result<SkillCreatedResponse>.Failure("Skill name is required");

                var existing = await skillRepository.GetByNameAsync(name);
                if (existing is not null)
                    return Result<SkillCreatedResponse>.Success(
                        new SkillCreatedResponse(existing.Id, existing.Name),
                        "Skill already exists");

                var skill = new Skill
                {
                    Name = name,
                    Description = string.Empty,
                    CreatedBy = request.CreatedBy,
                    DateCreated = DateTime.UtcNow
                };

                await skillRepository.AddAsync(skill);
                await unitOfWork.SaveAsync();

                return Result<SkillCreatedResponse>.Success(
                    new SkillCreatedResponse(skill.Id, skill.Name),
                    "Skill created");
            }
        }
    }
}