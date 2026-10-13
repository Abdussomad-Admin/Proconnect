using Application.Commands;
using Application.Common.Pagenation;
using Application.Queries.Search;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Authentication.UpdateUserBasicInfo;
using static Application.Commands.Authentication.UploadProfilePicture;
using static Application.Commands.Authentication.UploadProfilePicture.UploadProfilePictureHandler;
using static Application.Commands.Professional.AddCertificate;
using static Application.Commands.Professional.AddEducation;
using static Application.Commands.Professional.AddExperience;
using static Application.Commands.Professional.AddPortfolioLink;
using static Application.Commands.Professional.AddProfessionalSkill;
using static Application.Commands.Professional.AddProject;
using static Application.Commands.Professional.DeleteCertificate;
using static Application.Commands.Professional.DeleteEducation;
using static Application.Commands.Professional.DeleteExperience;
using static Application.Commands.Professional.DeleteProject;
using static Application.Commands.Professional.DeleteResume;
using static Application.Commands.Professional.RemoveProfessionalSkill;
using static Application.Commands.Professional.TrackResumeDownload;
using static Application.Commands.Professional.TrackResumeView;
using static Application.Commands.Professional.UpdateAvailability;
using static Application.Commands.Professional.UpdateCertificate;
using static Application.Commands.Professional.UpdateEducation;
using static Application.Commands.Professional.UpdateExperience;
using static Application.Commands.Professional.UpdatePortfolioLink;
using static Application.Commands.Professional.UpdateProfessionalProfile;
using static Application.Commands.Professional.UpdateProject;
using static Application.Commands.Professional.UploadResume;
using static Application.Commands.Professional.CreateSkill;
using static Application.Commands.UploadCompanyLogo.UploadCompanyLogoHandler;
using static Application.Queries.Professional.GetCertificateById;
using static Application.Queries.Professional.GetCertificatesByProfile;
using static Application.Queries.Professional.GetEducationById;
using static Application.Queries.Professional.GetEducationsByProfile;
using static Application.Queries.Professional.GetExperienceById;
using static Application.Queries.Professional.GetExperiencesByProfile;
using static Application.Queries.Professional.GetPortfolioLinksByProfile;
using static Application.Queries.Professional.GetProfessionalProfile;
using static Application.Queries.Professional.GetProfessionalSkillById;
using static Application.Queries.Professional.GetProfessionalSkillsByProfile;
using static Application.Queries.Professional.GetProjectById;
using static Application.Queries.Professional.GetProjectsByProfile;
using static Application.Queries.Skills.GetSkills;

namespace Host.Controllers.Professional
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalController(IMediator mediator) : ControllerBase
    {
        [HttpPost("update-professional-profile")]

        public async Task<IActionResult> UpdateProfile(UpdateProfessionalProfileCommand profile)
        {
            var response = await mediator.Send(profile);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-experience")]
        public async Task<IActionResult> AddExperience([FromBody] AddExperienceCommand experience)
        {
            var response = await mediator.Send(experience);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpPost("add-education")]
        public async Task<IActionResult> AddEducation(AddEducationCommand education)
        {
            var response = await mediator.Send(education);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-certificate")]
        public async Task<IActionResult> AddCertificate(AddCertificateCommand certificate)
        {
            var response = await mediator.Send(certificate);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-professional-skills")]
        public async Task<IActionResult> AddSkills(AddProfessionalSkillCommand skills)
        {
            var response = await mediator.Send(skills);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-project")]

        public async Task<IActionResult> AddProject(AddProjectCommand project)
        {
            var response = await mediator.Send(project);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-education")]

        public async Task<IActionResult> UpdateEducation(UpdateEducationCommand education)
        {
            var response = await mediator.Send(education);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("delete-education")]

        public async Task<IActionResult> DeleteEducation(DeleteEducationCommand education)
        {
            var response = await mediator.Send(education);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-experience")]

        public async Task<IActionResult> UpdateExperience(UpdateExperienceCommand experience)
        {
            var response = await mediator.Send(experience);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("delete-experience")]

        public async Task<IActionResult> DeleteExperience(DeleteExperienceCommand experience)
        {
            var response = await mediator.Send(experience);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("remove-professional-skill")]

        public async Task<IActionResult> RemoveSkill(RemoveProfessionalSkillCommand skill)
        {
            var response = await mediator.Send(skill);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(Guid id, [FromQuery] string? referer)
        {
            Guid? viewerUserId = User.Identity?.IsAuthenticated == true
                ? ClaimsHelper.GetUserId(User)
                : null;

            var response = await mediator.Send(new GetProfessionalProfileQuery(id, viewerUserId, ReferrerHelper.Parse(referer)));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("education/{id}")]

        public async Task<IActionResult> GetEducation(Guid id)
        {
            var response = await mediator.Send(new GetEducationByIdQuery(id));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("educations")]

        public async Task<IActionResult> GetEducations(
            [FromQuery] Guid professionalProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetEducationsByProfileQuery(professionalProfileId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("experience/{id}")]

        public async Task<IActionResult> GetExperience(Guid id)
        {
            var response = await mediator.Send(new GetExperienceByIdQuery(id));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("experiences")]

        public async Task<IActionResult> GetExperiences(
            [FromQuery] Guid professionalProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetExperiencesByProfileQuery(professionalProfileId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("certificate/{id}")]

        public async Task<IActionResult> GetCertificate(Guid id)
        {
            var response = await mediator.Send(new GetCertificateByIdQuery(id));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("certificates")]

        public async Task<IActionResult> GetCertificates(
            [FromQuery] Guid professionalProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetCertificatesByProfileQuery(professionalProfileId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("project/{id}")]

        public async Task<IActionResult> GetProject(Guid id)
        {
            var response = await mediator.Send(new GetProjectByIdQuery(id));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("projects")]

        public async Task<IActionResult> GetProjects(
            [FromQuery] Guid professionalProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetProjectsByProfileQuery(professionalProfileId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("professional-skill/{id}")]

        public async Task<IActionResult> GetProfessionalSkill(Guid id)
        {
            var response = await mediator.Send(new GetProfessionalSkillByIdQuery(id));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("professional-skills")]

        public async Task<IActionResult> GetProfessionalSkills(
            [FromQuery] Guid professionalProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetProfessionalSkillsByProfileQuery(professionalProfileId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-certificate")]

        public async Task<IActionResult> UpdateCertificate(UpdateCertificateCommand certificate)
        {
            var response = await mediator.Send(certificate);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("delete-certificate")]

        public async Task<IActionResult> DeleteCertificate(DeleteCertificateCommand certificate)
        {
            var response = await mediator.Send(certificate);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-project")]

        public async Task<IActionResult> UpdateProject(UpdateProjectCommand project)
        {
            var response = await mediator.Send(project);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("delete-project")]

        public async Task<IActionResult> DeleteProject(DeleteProjectCommand project)
        {
            var response = await mediator.Send(project);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("upload-resume")]

        public async Task<IActionResult> UploadResume([FromForm] UploadResumeCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-portfolio-link")]

        public async Task<IActionResult> AddPortfolioLink([FromForm] AddPortfolioLinkCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-portfolio-link")]

        public async Task<IActionResult> UpdatePortfolioLink([FromForm] UpdatePortfolioLinkCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-availability")]

        public async Task<IActionResult> UpdateAvailability(UpdateAvailabilityCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("track-resume-view")]

        public async Task<IActionResult> TrackResumeView(TrackResumeViewCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("track-resume-download")]

        public async Task<IActionResult> TrackResumeDownload(TrackResumeDownloadCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("portfolio-links")]

        public async Task<IActionResult> GetPortfolioLinks(
            [FromQuery] Guid professionalProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetPortfolioLinksByProfileQuery(professionalProfileId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePics([FromForm] UploadProfilePictureRequest request)
        {
            var command = new UploadProfilePictureCommand(
                ClaimsHelper.GetUserId(User),
                request.File);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpPut("logo")]

        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCompanyLogo([FromForm] UploadCompanyLogoDto logoDto)
        {
            var result = await mediator.Send(
                new UploadCompanyLogo.UploadCompanyLogoCommand(ClaimsHelper.GetUserId(User), logoDto.File));

            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update-basic-info")]

        public async Task<IActionResult> UpdateBasicInfo(UpdateUserBasicInfoCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("delete-resume")]

        public async Task<IActionResult> DeleteResume(DeleteResumeCommand command)
        {
            var response = await mediator.Send(command);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("skills")]

        public async Task<IActionResult> GetSkills()
        {
            var response = await mediator.Send(new GetSkillsQuery());

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var result = await mediator.Send(new SearchSkills.SearchSkillsQuery(query));
            return result.Status ? Ok(result) : BadRequest(result);
        }


        [HttpPost("skills")]
        public async Task<IActionResult> CreateSkill(CreateSkillCommand command)
        {
            var response = await mediator.Send(command with {CreatedBy = ClaimsHelper.GetUserId(User).ToString() });

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
