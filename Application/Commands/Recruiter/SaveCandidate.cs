using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.SavedCandidates
{
    public class SaveCandidate
    {
        public record SaveCandidateCommand(
            Guid RequestingUserId,
            Guid ProfessionalProfileId
        ) : IRequest<Result<string>>;

        public class SaveCandidateHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            ISavedCandidateRepository savedCandidateRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<SaveCandidateCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(SaveCandidateCommand request, CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.RequestingUserId);

                if (recruiterProfile is null)
                    return Result<string>.Failure("You are not a recruiter");

                var exists = await savedCandidateRepository.ExistsAsync(recruiterProfile.Id, request.ProfessionalProfileId);

                if (exists)
                    return Result<string>.Failure("Candidate already saved");

                var savedCandidate = new SavedCandidate
                {
                    RecruiterProfileId = recruiterProfile.Id,
                    ProfessionalProfileId = request.ProfessionalProfileId,
                    CreatedBy = recruiterProfile.Id.ToString(),
                    DateCreated = DateTime.UtcNow
                };

                await savedCandidateRepository.AddAsync(savedCandidate);
                await unitOfWork.SaveAsync();

                return Result<string>.Success("Candidate saved", "saved");
            }
        }
    }
}