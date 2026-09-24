using Application.Common.Dtos;
using Application.Common.Filters;
using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories;

public interface IProfessionalProfileRepository
{
    Task AddAsync(ProfessionalProfile profile);

    Task<ProfessionalProfile?> GetByIdAsync(Guid id);

    Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId);

    Task<ProfessionalProfile?> GetWithDetailsAsync(Guid id);

    Task<PageResponse<ProfessionalProfile>> GetAllAsync(PageRequest request, bool usePaging);

    Task<PageResponse<ProfessionalProfile>> SearchAsync(
        TalentSearchFilter filter,
        PageRequest request,
        bool usePaging);

    Task<TalentStatsDto> GetTalentStatsAsync();

    void Update(ProfessionalProfile profile);

    void Delete(ProfessionalProfile profile);
}