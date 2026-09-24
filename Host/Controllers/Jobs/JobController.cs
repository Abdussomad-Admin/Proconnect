using Application.Commands.Job;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Job.AddJobSkill;
using static Application.Commands.Job.ApplyJob;
using static Application.Commands.Job.CloseJob;
using static Application.Commands.Job.CreateJob;
using static Application.Commands.Job.CreateJobAlert;
using static Application.Commands.Job.DeleteJob;
using static Application.Commands.Job.DeleteJobAlert;
using static Application.Commands.Job.PublishJob;
using static Application.Commands.Job.RemoveJobSkill;
using static Application.Commands.Job.SaveJob;
using static Application.Commands.Job.ScheduleInterview;
using static Application.Commands.Job.ToggleJobAlert;
using static Application.Commands.Job.UnsaveJob;
using static Application.Commands.Job.UpdateApplicationStatus;
using static Application.Commands.Job.UpdateJob;
using static Application.Commands.Job.WithdrawApplication;
using static Application.Queries.Job.GetApplicationsByJob;
using static Application.Queries.Job.GetApplicationsByProfessional;
using static Application.Queries.Job.GetApplicationsByRecruiter;
using static Application.Queries.Job.GetApplicationStatusCounts;
using static Application.Queries.Job.GetApplicationStatusCountsByProfessional;
using static Application.Queries.Job.GetJobAlerts;
using static Application.Queries.Job.GetJobById;
using static Application.Queries.Job.GetJobsByRecruiter;
using static Application.Queries.Job.GetJobSkills;
using static Application.Queries.Job.GetRecommendedJobs;
using static Application.Queries.Job.GetSavedJobsByProfessional;
using static Application.Queries.Job.SearchJobs;

namespace Host.Controllers.Job
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController(IMediator mediator) : ControllerBase
    {
        [HttpPost("create-job")]

        public async Task<IActionResult> CreateJob(CreateJobCommand job)
        {
            var response = await mediator.Send(job);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-job")]

        public async Task<IActionResult> UpdateJob(UpdateJobCommand job)
        {
            var response = await mediator.Send(job);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("publish-job")]

        public async Task<IActionResult> PublishJob(PublishJobCommand job)
        {
            var response = await mediator.Send(job);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("close-job")]

        public async Task<IActionResult> CloseJob(CloseJobCommand job)
        {
            var response = await mediator.Send(job);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("delete-job")]

        public async Task<IActionResult> DeleteJob(DeleteJobCommand job)
        {
            var response = await mediator.Send(job);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("apply-job")]

        public async Task<IActionResult> ApplyJob(ApplyJobCommand application)
        {
            var response = await mediator.Send(application);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-application-status")]

        public async Task<IActionResult> UpdateApplicationStatus(UpdateApplicationStatusCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("save-job")]

        public async Task<IActionResult> SaveJob(SaveJobCommand job)
        {
            var response = await mediator.Send(job);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("unsave-job")]

        public async Task<IActionResult> UnsaveJob(UnsaveJobCommand job)
        {
            var response = await mediator.Send(job);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        

        [HttpGet("job/{id}")]
        public async Task<IActionResult> GetJob(Guid id, [FromQuery] string? @ref)
        {
            Guid? viewerUserId = User.Identity?.IsAuthenticated == true
                ? ClaimsHelper.GetUserId(User)
                : null;

            var response = await mediator.Send(new GetJobByIdQuery(id, viewerUserId, ReferrerHelper.Parse(@ref)));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("search")]

        public async Task<IActionResult> SearchJobs(
            [FromQuery] string? keyword,
            [FromQuery] Guid? jobCategoryId,
            [FromQuery] Guid? companyId,
            [FromQuery] string? location,
            [FromQuery] EmploymentType? employmentType,
            [FromQuery] WorkPlaceType? workPlaceType,
            [FromQuery] ExperienceLevel? experienceLevel,
            [FromQuery] decimal? minSalary,
            [FromQuery] decimal? maxSalary,
            [FromQuery] DateTime? postedAfter,
            [FromQuery] JobSortOption sortBy = JobSortOption.Newest,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(new SearchJobsQuery(
                keyword,
                jobCategoryId,
                companyId,
                location,
                employmentType,
                workPlaceType,
                experienceLevel,
                minSalary,
                maxSalary,
                postedAfter,
                sortBy,
                pageRequest,
                usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("recruiter-jobs")]

        public async Task<IActionResult> GetJobsByRecruiter(
            [FromQuery] Guid recruiterProfileId,
            [FromQuery] JobPostingStatus? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetJobsByRecruiterQuery(recruiterProfileId, status, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("saved-jobs")]

        public async Task<IActionResult> GetSavedJobs(
            [FromQuery] Guid professionalProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetSavedJobsByProfessionalQuery(professionalProfileId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("applications/job/{jobId}")]

        public async Task<IActionResult> GetApplicationsByJob(
            Guid jobId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetApplicationsByJobQuery(jobId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("applications/professional")]

        public async Task<IActionResult> GetApplicationsByProfessional(
            [FromQuery] Guid professionalProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetApplicationsByProfessionalQuery(professionalProfileId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("job-categories")]

    
        [HttpGet("applications/recruiter")]

        public async Task<IActionResult> GetApplicationsByRecruiter(
     [FromQuery] Guid recruiterProfileId,
     [FromQuery] JobStatus? status,
     [FromQuery] Guid? jobId,
     [FromQuery] string? keyword,
     [FromQuery] DateTime? appliedAfter,
     [FromQuery] DateTime? appliedBefore,
     [FromQuery] WorkAuthorizationStatus? workAuthorization,
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(new GetApplicationsByRecruiterQuery(
                recruiterProfileId, status, jobId, keyword, appliedAfter, appliedBefore, workAuthorization,
                pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("applications/recruiter/status-counts")]

        public async Task<IActionResult> GetApplicationStatusCounts(
            [FromQuery] Guid recruiterProfileId,
            [FromQuery] Guid? jobId,
            [FromQuery] string? keyword,
            [FromQuery] DateTime? appliedAfter,
            [FromQuery] DateTime? appliedBefore,
            [FromQuery] WorkAuthorizationStatus? workAuthorization)
        {
            var response = await mediator.Send(new GetApplicationStatusCountsQuery(
                recruiterProfileId, jobId, keyword, appliedAfter, appliedBefore, workAuthorization));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("schedule-interview")]

        public async Task<IActionResult> ScheduleInterview(ScheduleInterviewCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("upload-application-resume")]

        public async Task<IActionResult> UploadApplicationResume([FromForm] UploadApplicationResume.UploadApplicationResumeCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-job-skill")]

        public async Task<IActionResult> AddJobSkill(AddJobSkillCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("remove-job-skill")]

        public async Task<IActionResult> RemoveJobSkill(RemoveJobSkillCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("job-skills/{jobId}")]

        public async Task<IActionResult> GetJobSkills(Guid jobId)
        {
            var response = await mediator.Send(new GetJobSkillsQuery(jobId));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("applications/professional/status-counts")]

        public async Task<IActionResult> GetApplicationStatusCountsByProfessional([FromQuery] Guid professionalProfileId)
        {
            var response = await mediator.Send(new GetApplicationStatusCountsByProfessionalQuery(professionalProfileId));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("withdraw-application")]

        public async Task<IActionResult> WithdrawApplication(WithdrawApplicationCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create-job-alert")]
        public async Task<IActionResult> CreateJobAlert(CreateJobAlertCommand command)
        {
            var response = await mediator.Send(command);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("delete-job-alert")]
        public async Task<IActionResult> DeleteJobAlert(DeleteJobAlertCommand command)
        {
            var response = await mediator.Send(command);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("toggle-job-alert")]
        public async Task<IActionResult> ToggleJobAlert(ToggleJobAlertCommand command)
        {
            var response = await mediator.Send(command);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("job-alerts")]
        public async Task<IActionResult> GetJobAlerts([FromQuery] Guid professionalProfileId)
        {
            var response = await mediator.Send(new GetJobAlertsQuery(professionalProfileId));
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommendedJobs(
        [FromQuery] Guid professionalProfileId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };
            var response = await mediator.Send(new GetRecommendedJobsQuery(professionalProfileId, pageRequest, usePaging));
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }
    }
}

