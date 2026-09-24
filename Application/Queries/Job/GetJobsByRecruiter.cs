using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetJobsByRecruiter
    {
        public record GetJobsByRecruiterQuery(
            Guid RecruiterProfileId,
            JobPostingStatus? Status,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<GetJobsByRecruiterResponse>>>;

        public class GetJobsByRecruiterHandler(
            IJobRepository jobRepository) : IRequestHandler<GetJobsByRecruiterQuery, Result<PageResponse<GetJobsByRecruiterResponse>>>
        {
            public async Task<Result<PageResponse<GetJobsByRecruiterResponse>>> Handle(
                GetJobsByRecruiterQuery request,
                CancellationToken cancellationToken)
            {
                var jobs = await jobRepository.GetByRecruiterProfileIdAsync(
                    request.RecruiterProfileId, request.Status, request.PageRequest, request.UsePaging);

                var response = new PageResponse<GetJobsByRecruiterResponse>
                {
                    Items = jobs.Items.Select(j => new GetJobsByRecruiterResponse(
                        j.Id,
                        j.JobCategoryId,
                        j.Department.Name,
                        j.Title,
                        j.EmploymentType,
                        j.WorkPlaceType,
                        j.ExperienceLevel,
                        j.Location,
                        j.ApplicationDeadline,
                        j.Status,
                        j.JobApplications.Count,
                        j.DateCreated)).ToList(),
                    TotalCount = jobs.TotalCount,
                    PageNumber = jobs.PageNumber,
                    PageSize = jobs.PageSize
                };

                return Result<PageResponse<GetJobsByRecruiterResponse>>.Success(response, "Jobs retrieved successfully");
            }
        }
    }

    public record GetJobsByRecruiterResponse(
        Guid Id,
        Guid JobCategoryId,
        string CategoryName,
        string Title,
        EmploymentType EmploymentType,
        WorkPlaceType WorkPlaceType,
        ExperienceLevel ExperienceLevel,
        string Location,
        DateTime ApplicationDeadline,
        JobPostingStatus Status,
        int ApplicantCount,
        DateTime DateCreated);
}