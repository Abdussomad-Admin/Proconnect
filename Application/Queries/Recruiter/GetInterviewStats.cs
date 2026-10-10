using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Recruiter
{
    public class GetInterviewStats
    {
        public record InterviewStatsResponse(
            int Total,
            int Upcoming,
            int Past,
            int RejectedAfterScheduling,
            int DueTodayOrTomorrow,
            double? TotalTrendPercent 
        );

        public record GetInterviewStatsQuery(Guid RequestingUserId) : IRequest<Result<InterviewStatsResponse>>;

        public class GetInterviewStatsHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetInterviewStatsQuery, Result<InterviewStatsResponse>>
        {
            public async Task<Result<InterviewStatsResponse>> Handle(
                GetInterviewStatsQuery request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.RequestingUserId);

                if (recruiterProfile is null)
                    return Result<InterviewStatsResponse>.Failure("You are not a recruiter");

                var counts = await jobApplicationRepository.GetInterviewCountsByRecruiterAsync(recruiterProfile.Id);

                var now = DateTime.UtcNow;
                var periodStart = now.AddDays(-7);
                var previousPeriodStart = now.AddDays(-14);

                var trend = await jobApplicationRepository.GetInterviewSchedulingTrendByRecruiterAsync(
                    recruiterProfile.Id, periodStart, previousPeriodStart, periodStart);

                double? trendPercent = trend.PreviousPeriodCount == 0
                    ? null
                    : Math.Round(((double)(trend.CurrentPeriodCount - trend.PreviousPeriodCount) / trend.PreviousPeriodCount) * 100, 0);

                var response = new InterviewStatsResponse(
                    counts.Total,
                    counts.Upcoming,
                    counts.Past,
                    counts.RejectedAfterScheduling,
                    counts.DueTodayOrTomorrow,
                    trendPercent
                );

                return Result<InterviewStatsResponse>.Success(response, "Interview stats retrieved");
            }
        }
    }
}