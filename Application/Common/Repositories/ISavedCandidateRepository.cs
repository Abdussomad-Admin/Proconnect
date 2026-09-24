using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ISavedCandidateRepository
    {
        Task<bool> ExistsAsync(Guid recruiterProfileId, Guid professionalProfileId);
        Task<SavedCandidate?> GetByRecruiterAndProfileAsync(Guid recruiterProfileId, Guid professionalProfileId);
        Task AddAsync(SavedCandidate savedCandidate);
        void Delete(SavedCandidate savedCandidate);
        Task<PageResponse<SavedCandidate>> GetByRecruiterIdAsync(Guid recruiterProfileId, PageRequest request, bool usePaging);
    }
}