using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.SavedCandidates
{
    public class RemoveSavedCandidate
    {
        public record RemoveSavedCandidateCommand(
            Guid RequestingUserId,
            Guid ProfessionalProfileId
        ) : IRequest<Result<string>>;

        public class RemoveSavedCandidateHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            ISavedCandidateRepository savedCandidateRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<RemoveSavedCandidateCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(RemoveSavedCandidateCommand request, CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.RequestingUserId);

                if (recruiterProfile is null)
                    return Result<string>.Failure("You are not a recruiter");

                var savedCandidate = await savedCandidateRepository.GetByRecruiterAndProfileAsync(recruiterProfile.Id, request.ProfessionalProfileId);

                if (savedCandidate is null)
                    return Result<string>.Failure("Candidate is not saved");

                savedCandidateRepository.Delete(savedCandidate);
                await unitOfWork.SaveAsync();

                return Result<string>.Success("Candidate removed from saved list", "removed");
            }
        }
    }
}