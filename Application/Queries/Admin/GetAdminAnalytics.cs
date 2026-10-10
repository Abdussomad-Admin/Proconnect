using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminAnalytics
    {
        public record GetAdminAnalyticsQuery(DateTime? From, DateTime? To)
            : IRequest<Result<AdminAnalyticsResponse>>;

        public class GetAdminAnalyticsHandler(IAdminDashboardRepository repository)
            : IRequestHandler<GetAdminAnalyticsQuery, Result<AdminAnalyticsResponse>>
        {
            private const int TopCategoryCount = 5;
            private const int TopCompanyCount = 5;
            private const int RecentActivityCount = 5;

            public async Task<Result<AdminAnalyticsResponse>> Handle(
                GetAdminAnalyticsQuery request, CancellationToken cancellationToken)
            {
                var today = DateTime.UtcNow.Date;
                var start = (request.From ?? today.AddDays(-29)).Date;
                var lastDay = (request.To ?? today).Date;

                if (lastDay < start)
                    return Result<AdminAnalyticsResponse>.Failure("The end date must not be before the start date");

                var end = lastDay.AddDays(1);

                var totals = await repository.GetTotalsAsync(cancellationToken);
                var registrations = await repository.GetRegistrationsByDayAsync(start, end, cancellationToken);
                var distribution = await repository.GetUserDistributionAsync(cancellationToken);
                var topCategories = await repository.GetTopJobCategoriesAsync(TopCategoryCount, cancellationToken);
                var reportsByReason = await repository.GetReportsByReasonAsync(start, end, cancellationToken);
                var platformActivity = await repository.GetPlatformActivityByDayAsync(start, end, cancellationToken);
                var topCompanies = await repository.GetTopCompaniesByActivityAsync(start, end, TopCompanyCount, cancellationToken);
                var recentActivity = await repository.GetRecentActivityAsync(RecentActivityCount, cancellationToken);

                var distributionTotal = distribution.Sum(d => d.Count);
                var distributionItems = distribution
                    .Select(d => new UserDistributionItem(d.Role, d.Count, distributionTotal == 0 ? 0 : Math.Round((decimal)d.Count / distributionTotal * 100, 1)))
                    .ToList();

                var reportsTotal = reportsByReason.Sum(r => r.Count);
                var reportsByReasonItems = reportsByReason
                    .Select(r => new ReasonBreakdownItem(r.Reason, r.Count, reportsTotal == 0 ? 0 : Math.Round((decimal)r.Count / reportsTotal * 100, 1)))
                    .ToList();

                var response = new AdminAnalyticsResponse(
                    start,
                    lastDay,
                    totals.TotalUsers,
                    totals.Companies,
                    totals.ActiveJobs,
                    totals.Posts,
                    totals.PendingReports,
                    registrations,
                    distributionItems,
                    topCategories,
                    reportsByReasonItems,
                    platformActivity,
                    topCompanies,
                    recentActivity);

                return Result<AdminAnalyticsResponse>.Success(response, "Analytics retrieved successfully");
            }
        }
    }

    public record ReasonBreakdownItem(string Reason, int Count, decimal Percent);

    public record AdminAnalyticsResponse(
        DateTime From,
        DateTime To,
        int TotalUsers,
        int TotalCompanies,
        int TotalJobsPosted,
        int TotalPosts,
        int TotalReports,
        List<DailyRegistrationsRow> UserGrowth,
        List<UserDistributionItem> UserDistribution,
        List<CategoryCountRow> TopJobCategories,
        List<ReasonBreakdownItem> ReportsByReason,
        List<DailyPlatformActivityRow> PlatformActivity,
        List<TopCompanyActivityRow> TopCompaniesByActivity,
        List<RecentActivityRow> RecentActivity);
}