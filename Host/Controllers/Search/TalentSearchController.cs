using Application.Queries;
using Application.Queries.Search;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Host.Controllers.Search
{
    // ASSUMPTION: adjust [Authorize(Roles = "...")] to whatever role string
    // your JWT claims actually use for recruiters — not shown in anything
    // pasted so far.
    [ApiController]
    [Route("api/talent-search")]
    [Authorize(Roles = "Recruiter")]
    public class TalentSearchController(IMediator mediator) : ControllerBase
    {
        public record SearchCandidatesRequest(
            List<Guid>? SkillIds,
            int? MinYearsExperience,
            int? MaxYearsExperience,
            string? EducationLevel,
            string? Location,
            AvailabilityStatus? Availability,
            string? Keyword,
            int PageNumber = 1,
            int PageSize = 20,
            bool UsePaging = true
        );

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchCandidatesRequest request)
        {
            var query = new SearchCandidates.SearchCandidatesQuery(
                GetUserId(),
                request.SkillIds,
                request.MinYearsExperience,
                request.MaxYearsExperience,
                request.EducationLevel,
                request.Location,
                request.Availability,
                request.Keyword,
                request.PageNumber,
                request.PageSize,
                request.UsePaging);

            var result = await mediator.Send(query);

            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("insights")]
        public async Task<IActionResult> Insights()
        {
            var result = await mediator.Send(new GetTalentInsights.GetTalentInsightsQuery(GetUserId()));
            return result.Status ? Ok(result) : BadRequest(result);
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}