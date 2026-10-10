using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Queries.Analytics;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController(
        IMediator mediator,
        IProfessionalProfileRepository professionalProfileRepository) : ControllerBase
    {
        [HttpGet("professional/dashboard")]
        public async Task<IActionResult> GetProfessionalDashboard(
            [FromQuery] DateRangePreset preset = DateRangePreset.Last30Days,
            [FromQuery] DateTime? customStart = null,
            [FromQuery] DateTime? customEnd = null)
        {
            var userId = ClaimsHelper.GetUserId(User);


            var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(userId);

            if (professionalProfile is null)
                return BadRequest(Application.Common.Dtos.Result<object>.Failure("No professional profile found for this account"));

            var query = new GetProfessionalAnalyticsDashboard.GetProfessionalAnalyticsDashboardQuery(
                professionalProfile.Id,
                userId,
                preset,
                customStart,
                customEnd);

            var response = await mediator.Send(query);

            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("recruiter/dashboard")]
        public async Task<IActionResult> GetRecruiterDashboard(
    [FromQuery] Guid recruiterProfileId,
    [FromQuery] DateRangePreset preset = DateRangePreset.Last30Days,
    [FromQuery] DateTime? customStart = null,
    [FromQuery] DateTime? customEnd = null)
        {
            var query = new GetRecruiterAnalyticsDashboard.GetRecruiterAnalyticsDashboardQuery(
                recruiterProfileId, preset, customStart, customEnd);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("recruiter/jobs")]
        public async Task<IActionResult> GetJobAnalytics(
            [FromQuery] Guid recruiterProfileId,
            [FromQuery] DateRangePreset preset = DateRangePreset.Last30Days,
            [FromQuery] DateTime? customStart = null,
            [FromQuery] DateTime? customEnd = null,
            [FromQuery] Guid? jobId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var query = new GetJobAnalytics.GetJobAnalyticsQuery(
                recruiterProfileId, preset, customStart, customEnd, jobId,
                new Application.Common.Pagenation.PageRequest { PageNumber = pageNumber, PageSize = pageSize },
                usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("recruiter/funnel")]
        public async Task<IActionResult> GetApplicationFunnel(
            [FromQuery] Guid recruiterProfileId,
            [FromQuery] DateRangePreset preset = DateRangePreset.Last30Days,
            [FromQuery] DateTime? customStart = null,
            [FromQuery] DateTime? customEnd = null,
            [FromQuery] Guid? jobId = null)
        {
            var query = new GetApplicationFunnelAnalytics.GetApplicationFunnelAnalyticsQuery(
                recruiterProfileId, preset, customStart, customEnd, jobId);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("professional/posts")]
        public async Task<IActionResult> GetProfessionalPostAnalytics(
    [FromQuery] DateRangePreset preset = DateRangePreset.Last30Days,
    [FromQuery] DateTime? customStart = null,
    [FromQuery] DateTime? customEnd = null)
        {
            var userId = ClaimsHelper.GetUserId(User);

            var query = new GetProfessionalPostAnalytics.GetProfessionalPostAnalyticsQuery(
                userId, preset, customStart, customEnd);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("professional/views")]
        public async Task<IActionResult> GetProfessionalViews(
    [FromQuery] DateRangePreset preset = DateRangePreset.Last30Days,
    [FromQuery] DateTime? customStart = null,
    [FromQuery] DateTime? customEnd = null)
        {
            var userId = ClaimsHelper.GetUserId(User);

            var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(userId);

            if (professionalProfile is null)
                return BadRequest(Application.Common.Dtos.Result<object>.Failure("No professional profile found for this account"));

            var query = new GetProfessionalViewsAnalytics.GetProfessionalViewsAnalyticsQuery(
                professionalProfile.Id, preset, customStart, customEnd);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("professional/demographics")]
        public async Task<IActionResult> GetProfileDemographics(
    [FromQuery] DateRangePreset preset = DateRangePreset.Last30Days,
    [FromQuery] DateTime? customStart = null,
    [FromQuery] DateTime? customEnd = null)
        {
            var userId = ClaimsHelper.GetUserId(User);

            var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(userId);

            if (professionalProfile is null)
                return BadRequest(Result<object>.Failure("No professional profile found for this account"));

            var query = new GetProfileDemographicsAnalytics.GetProfileDemographicsAnalyticsQuery(
                professionalProfile.Id, preset, customStart, customEnd);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }
    }
}