using Application.Commands.Admin;
using Application.Common.Pagenation;
using Application.Queries.Admin;
using Domain.Enums;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    [Authorize]
    public class AdminController(IMediator mediator) : ControllerBase
    {
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var query = new GetAdminDashboard.GetAdminDashboardQuery(from, to);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? type = null,
            [FromQuery] string? search = null,
            [FromQuery] UserStatus? status = null,
            [FromQuery] DateTime? registeredFrom = null,
            [FromQuery] DateTime? registeredTo = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var query = new GetAdminUsers.GetAdminUsersQuery(
                type, search, status, registeredFrom, registeredTo,
                new PageRequest { PageNumber = pageNumber, PageSize = pageSize }, usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("users/{userId:guid}")]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            var response = await mediator.Send(new GetAdminUserDetails.GetAdminUserDetailsQuery(userId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("users/{userId:guid}/restrict")]
        public async Task<IActionResult> RestrictUser(Guid userId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateUserStatus.UpdateUserStatusCommand(userId, adminId, UserStatus.Deactivated, "Restrict", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var response = await mediator.Send(new GetAdminProfile.GetAdminProfileQuery(adminId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] AdminProfileRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateAdminProfile.UpdateAdminProfileCommand(
                adminId, request.FirstName, request.LastName, request.Tel, request.Bio, request.Location);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("users/{userId:guid}/suspend")]
        public async Task<IActionResult> SuspendUser(Guid userId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateUserStatus.UpdateUserStatusCommand(userId, adminId, UserStatus.Suspended, "Suspend", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("users/{userId:guid}/restore")]
        public async Task<IActionResult> RestoreUser(Guid userId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateUserStatus.UpdateUserStatusCommand(userId, adminId, UserStatus.Active, "Restore", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("users/{userId:guid}/ban")]
        public async Task<IActionResult> BanUser(Guid userId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateUserStatus.UpdateUserStatusCommand(userId, adminId, UserStatus.Banned, "Ban", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies(
            [FromQuery] string? search = null,
            [FromQuery] string? industry = null,
            [FromQuery] string? location = null,
            [FromQuery] CompanyStatus? status = null,
            [FromQuery] DateTime? createdFrom = null,
            [FromQuery] DateTime? createdTo = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var query = new GetAdminCompanies.GetAdminCompaniesQuery(
                search, industry, location, status, createdFrom, createdTo,
                new PageRequest { PageNumber = pageNumber, PageSize = pageSize }, usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("companies/{companyId:guid}")]
        public async Task<IActionResult> GetCompanyDetails(Guid companyId)
        {
            var response = await mediator.Send(new GetAdminCompanyDetails.GetAdminCompanyDetailsQuery(companyId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("companies/{companyId:guid}/verify")]
        public async Task<IActionResult> VerifyCompany(Guid companyId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateCompanyStatus.UpdateCompanyStatusCommand(companyId, adminId, CompanyStatus.Active, "Verify", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("companies/{companyId:guid}/suspend")]
        public async Task<IActionResult> SuspendCompany(Guid companyId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateCompanyStatus.UpdateCompanyStatusCommand(companyId, adminId, CompanyStatus.Suspended, "Suspend", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("companies/{companyId:guid}/restore")]
        public async Task<IActionResult> RestoreCompany(Guid companyId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateCompanyStatus.UpdateCompanyStatusCommand(companyId, adminId, CompanyStatus.Active, "Restore", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("jobs/{jobId:guid}/approve")]
        public async Task<IActionResult> ApproveJob(Guid jobId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateJobStatus.UpdateJobStatusCommand(jobId, adminId, JobPostingStatus.Active, "Approve", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts(
    [FromQuery] string? search = null,
    [FromQuery] string? status = null,
    [FromQuery] DateTime? postedFrom = null,
    [FromQuery] DateTime? postedTo = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool usePaging = true)
        {
            var query = new GetAdminPosts.GetAdminPostsQuery(search, status, postedFrom, postedTo,
                new PageRequest { PageNumber = pageNumber, PageSize = pageSize }, usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("posts/{postId:guid}/remove")]
        public async Task<IActionResult> RemovePost(Guid postId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdatePostVisibility.UpdatePostVisibilityCommand(postId, adminId, true, "Remove", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("posts/{postId:guid}/restore")]
        public async Task<IActionResult> RestorePost(Guid postId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdatePostVisibility.UpdatePostVisibilityCommand(postId, adminId, false, "Restore", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports(
    [FromQuery] string? contentType = null,
    [FromQuery] ReportStatus? status = null,
    [FromQuery] ReportReason? reason = null,
    [FromQuery] string? search = null,
    [FromQuery] DateTime? createdFrom = null,
    [FromQuery] DateTime? createdTo = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool usePaging = true)
        {
            var query = new GetAdminReports.GetAdminReportsQuery(
                contentType, status, reason, search, createdFrom, createdTo,
                new PageRequest { PageNumber = pageNumber, PageSize = pageSize }, usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("reports/{reportId:guid}")]
        public async Task<IActionResult> GetReportDetails(Guid reportId)
        {
            var response = await mediator.Send(new GetAdminReportDetails.GetAdminReportDetailsQuery(reportId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("reports/{reportId:guid}/resolve")]
        public async Task<IActionResult> ResolveReport(Guid reportId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateReportStatus.UpdateReportStatusCommand(reportId, adminId, ReportStatus.Resolved, "Resolve", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("reports/{reportId:guid}/dismiss")]
        public async Task<IActionResult> DismissReport(Guid reportId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateReportStatus.UpdateReportStatusCommand(reportId, adminId, ReportStatus.Rejected, "Dismiss", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("reports/{reportId:guid}/escalate")]
        public async Task<IActionResult> EscalateReport(Guid reportId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateReportStatus.UpdateReportStatusCommand(reportId, adminId, ReportStatus.Escalated, "Escalate", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("activity-logs")]
        public async Task<IActionResult> GetActivityLogs(
    [FromQuery] string? search = null,
    [FromQuery] string? action = null,
    [FromQuery] Guid? userId = null,
    [FromQuery] DateTime? from = null,
    [FromQuery] DateTime? to = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool usePaging = true)
        {
            var query = new GetAdminActivityLogs.GetAdminActivityLogsQuery(
                search, action, userId, from, to,
                new PageRequest { PageNumber = pageNumber, PageSize = pageSize }, usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("jobs/{jobId:guid}/close")]
        public async Task<IActionResult> CloseJob(Guid jobId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateJobStatus.UpdateJobStatusCommand(jobId, adminId, JobPostingStatus.Closed, "Close", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("jobs")]
        public async Task<IActionResult> GetJobs(
    [FromQuery] string? search = null,
    [FromQuery] JobPostingStatus? status = null,
    [FromQuery] string? industry = null,
    [FromQuery] string? location = null,
    [FromQuery] DateTime? postedFrom = null,
    [FromQuery] DateTime? postedTo = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool usePaging = true)
        {
            var query = new GetAdminJobs.GetAdminJobsQuery(
                search, status, industry, location, postedFrom, postedTo,
                new PageRequest { PageNumber = pageNumber, PageSize = pageSize }, usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("jobs/{jobId:guid}")]
        public async Task<IActionResult> GetJobDetails(Guid jobId)
        {
            var response = await mediator.Send(new GetAdminJobDetails.GetAdminJobDetailsQuery(jobId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("system-health")]
        public async Task<IActionResult> GetSystemHealth()
        {
            var response = await mediator.Send(new GetAdminSystemHealth.GetAdminSystemHealthQuery());
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents(
    [FromQuery] string? search = null,
    [FromQuery] EventStatus? status = null,
    [FromQuery] string? location = null,
    [FromQuery] DateTime? startFrom = null,
    [FromQuery] DateTime? startTo = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool usePaging = true)
        {
            var query = new GetAdminEvents.GetAdminEventsQuery(search, status, location, startFrom, startTo,
                new PageRequest { PageNumber = pageNumber, PageSize = pageSize }, usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("events/{eventId:guid}")]
        public async Task<IActionResult> GetEventDetails(Guid eventId)
        {
            var response = await mediator.Send(new GetAdminEventDetails.GetAdminEventDetailsQuery(eventId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("events/{eventId:guid}/publish")]
        public async Task<IActionResult> PublishEvent(Guid eventId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateEventStatus.UpdateEventStatusCommand(eventId, adminId, EventStatus.Published, "Publish", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("events/{eventId:guid}/cancel")]
        public async Task<IActionResult> CancelEvent(Guid eventId, [FromBody] AdminRequest request)
        {
            var adminId = ClaimsHelper.GetUserId(User);
            var command = new UpdateEventStatus.UpdateEventStatusCommand(eventId, adminId, EventStatus.Cancelled, "Cancel", request.Reason);
            var response = await mediator.Send(command);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
        {
            var query = new GetAdminAnalytics.GetAdminAnalyticsQuery(from, to);
            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }
    }

    public record AdminRequest(string? Reason);
    public record AdminProfileRequest(string FirstName, string LastName, string? Tel, string? Bio, string? Location);
}