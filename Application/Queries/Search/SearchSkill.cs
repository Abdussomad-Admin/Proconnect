using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Search
{
    public class SearchSkills
    {
        public record SkillOptionResponse(Guid Id, string Name);

        public record SearchSkillsQuery(string Query) : IRequest<Result<List<SkillOptionResponse>>>;

        public class SearchSkillsHandler(ISkillRepository skillRepository)
            : IRequestHandler<SearchSkillsQuery, Result<List<SkillOptionResponse>>>
        {
            public async Task<Result<List<SkillOptionResponse>>> Handle(
                SearchSkillsQuery request,
                CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Query) || request.Query.Trim().Length < 2)
                    return Result<List<SkillOptionResponse>>.Success(new List<SkillOptionResponse>(), "Type at least 2 characters");

                var matches = await skillRepository.SearchByNameAsync(request.Query.Trim(), limit: 10);

                var response = matches.Select(s => new SkillOptionResponse(s.Id, s.Name)).ToList();

                return Result<List<SkillOptionResponse>>.Success(response, "Skills retrieved");
            }
        }
    }
}