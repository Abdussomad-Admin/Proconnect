using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetJobManagementOverview
    {
        public record GetJobManagementOverviewQuery(Guid RecruiterProfileId) : IRequest<Result<GetJobManagementOverviewResponse>>;

        public class GetJobManagementOverviewHandler(
            IJobRepository jobRepository,
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetJobManagementOverviewQuery, Result<GetJobManagementOverviewResponse>>
        {
            private static readonly HashSet<JobStatus> UnderReviewStatuses = new() { JobStatus.New, JobStatus.Screening, JobStatus.Shortlisted };
            private static readonly HashSet<JobStatus> OfferStatuses = new() { JobStatus.Offered, JobStatus.Hired };

            public async Task<Result<GetJobManagementOverviewResponse>> Handle(
                GetJobManagementOverviewQuery request,
                CancellationToken cancellationToken)
            {
                var pageRequest = new PageRequest { PageNumber = 1, PageSize = 1 };

                var jobsPage = await jobRepository.GetByRecruiterProfileIdAsync(
                    request.RecruiterProfileId, status: null, pageRequest, usePaging: false);

                var applicationsPage = await jobApplicationRepository.GetByRecruiterProfileIdAsync(
                    request.RecruiterProfileId,
                    new JobApplicationFilter(),
                    pageRequest,
                    usePaging: false);

                var jobs = jobsPage.Items;
                var applications = applicationsPage.Items;

                var applicationsByJobId = applications
                    .GroupBy(a => a.JobId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var activeJobPostCount = jobs.Count(j => j.Status == JobPostingStatus.Active);
                var totalApplicationCount = applications.Count();
                var underReviewCount = applications.Count(a => UnderReviewStatuses.Contains(a.JobStatus));
                var interviewsScheduledCount = applications.Count(a => a.InterviewScheduledAt.HasValue);
                var offersExtendedCount = applications.Count(a => OfferStatuses.Contains(a.JobStatus));

                var recentJobPosts = jobs
                    .OrderByDescending(j => j.DateCreated)
                    .Take(5)
                    .Select(j =>
                    {
                        var jobApps = applicationsByJobId.TryGetValue(j.Id, out var list) ? list : new List<Domain.Entities.JobApplication>();
                        return new RecentJobPostItem(
                            j.Id,
                            j.Title,
                            j.Status,
                            jobApps.Count,
                            jobApps.Count(a => UnderReviewStatuses.Contains(a.JobStatus)),
                            jobApps.Count(a => a.InterviewScheduledAt.HasValue),
                            jobApps.Count(a => OfferStatuses.Contains(a.JobStatus)),
                            j.DateCreated);
                    })
                    .ToList();

                var upcomingInterviews = applications
                    .Where(a => a.InterviewScheduledAt.HasValue && a.InterviewScheduledAt.Value >= DateTime.UtcNow)
                    .OrderBy(a => a.InterviewScheduledAt)
                    .Take(5)
                    .Select(a => new UpcomingInterviewItem(
                        a.Id,
                        $"{a.ProfessionalProfile.User.FirstName} {a.ProfessionalProfile.User.LastName}",
                        a.ProfessionalProfile.User.ProfilePictureUrl,
                        a.Job.Title,
                        a.InterviewScheduledAt!.Value,
                        a.InterviewType))
                    .ToList();

                var response = new GetJobManagementOverviewResponse(
                    activeJobPostCount,
                    totalApplicationCount,
                    underReviewCount,
                    interviewsScheduledCount,
                    offersExtendedCount,
                    recentJobPosts,
                    upcomingInterviews);

                return Result<GetJobManagementOverviewResponse>.Success(response, "Overview retrieved successfully");
            }
        }
    }

    public record RecentJobPostItem(
        Guid Id, string Title, JobPostingStatus Status,
        int ApplicationCount, int UnderReviewCount, int InterviewCount, int OfferCount,
        DateTime DateCreated);

    public record UpcomingInterviewItem(
        Guid ApplicationId, string CandidateName, string? CandidateAvatarUrl,
        string JobTitle, DateTime InterviewScheduledAt, string? InterviewType);

    public record GetJobManagementOverviewResponse(
        int ActiveJobPostCount,
        int TotalApplicationCount,
        int UnderReviewCount,
        int InterviewsScheduledCount,
        int OffersExtendedCount,
        List<RecentJobPostItem> RecentJobPosts,
        List<UpcomingInterviewItem> UpcomingInterviews);
}