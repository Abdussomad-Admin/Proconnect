using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminDashboard
    {
        public record GetAdminDashboardQuery(DateTime? From, DateTime? To)
            : IRequest<Result<AdminDashboardResponse>>;

        public class GetAdminDashboardHandler(IAdminDashboardRepository repository)
            : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardResponse>>
        {
            private const int MaxRangeDays = 366;
            private const int RecentItemCount = 5;
            private const int TopReportCount = 6;

            public async Task<Result<AdminDashboardResponse>> Handle(
                GetAdminDashboardQuery request,
                CancellationToken cancellationToken)
            {
                var today = DateTime.UtcNow.Date;
                var start = (request.From ?? today.AddDays(-6)).Date;
                var lastDay = (request.To ?? today).Date;

                if (lastDay < start)
                    return Result<AdminDashboardResponse>.Failure("The end date must not be before the start date");

                var end = lastDay.AddDays(1);
                var days = (end - start).Days;

                if (days > MaxRangeDays)
                    return Result<AdminDashboardResponse>.Failure($"The date range cannot exceed {MaxRangeDays} days");

                var previousStart = start.AddDays(-days);

                var totals = await repository.GetTotalsAsync(cancellationToken);
                var created = await repository.GetCreatedBetweenAsync(start, end, cancellationToken);
                var createdPrevious = await repository.GetCreatedBetweenAsync(previousStart, start, cancellationToken);

                var stats = new AdminDashboardStats(
                    TotalUsers: Stat(totals.TotalUsers, created.TotalUsers, createdPrevious.TotalUsers),
                    ActiveUsers: Stat(totals.ActiveUsers, created.ActiveUsers, createdPrevious.ActiveUsers),
                    Professionals: Stat(totals.Professionals, created.Professionals, createdPrevious.Professionals),
                    Recruiters: Stat(totals.Recruiters, created.Recruiters, createdPrevious.Recruiters),
                    Companies: Stat(totals.Companies, created.Companies, createdPrevious.Companies),
                    ActiveJobs: Stat(totals.ActiveJobs, created.ActiveJobs, createdPrevious.ActiveJobs),
                    ActiveEvents: Stat(totals.ActiveEvents, created.ActiveEvents, createdPrevious.ActiveEvents),
                    Posts: Stat(totals.Posts, created.Posts, createdPrevious.Posts),
                    PendingReports: Stat(totals.PendingReports, created.PendingReports, createdPrevious.PendingReports),
                    RestrictedAccounts: Stat(totals.RestrictedAccounts, created.RestrictedAccounts, createdPrevious.RestrictedAccounts));

                var registrationRows = (await repository.GetRegistrationsByDayAsync(start, end, cancellationToken))
                    .ToDictionary(r => r.Date.Date);

                var contentRows = (await repository.GetContentActivityByDayAsync(start, end, cancellationToken))
                    .ToDictionary(r => r.Date.Date);

                var dates = Enumerable.Range(0, days).Select(i => start.AddDays(i)).ToList();

                var registrations = dates
                    .Select(d => registrationRows.TryGetValue(d, out var row)
                        ? row with { Date = d }
                        : new DailyRegistrationsRow(d, 0, 0, 0, 0))
                    .ToList();

                var contentActivity = dates
                    .Select(d => contentRows.TryGetValue(d, out var row)
                        ? row with { Date = d }
                        : new DailyContentActivityRow(d, 0, 0, 0))
                    .ToList();

                var distributionRows = await repository.GetUserDistributionAsync(cancellationToken);
                var distributionTotal = distributionRows.Sum(r => r.Count);
                var distribution = distributionRows
                    .Select(r => new UserDistributionItem(
                        r.Role,
                        r.Count,
                        distributionTotal == 0 ? 0 : Math.Round((decimal)r.Count / distributionTotal * 100, 1)))
                    .ToList();

                var recentRegistrations = await repository.GetRecentRegistrationsAsync(RecentItemCount, cancellationToken);
                var recentActivity = await repository.GetRecentActivityAsync(RecentItemCount, cancellationToken);
                var topReports = await repository.GetTopReportsAsync(start, end, TopReportCount, cancellationToken);

                var databaseUp = await repository.IsDatabaseReachableAsync(cancellationToken);
                var health = new SystemHealthDto(
                    databaseUp ? "Operational" : "Degraded",
                    [
                        new ServiceHealthDto("API", "Operational"),
                        new ServiceHealthDto("Database", databaseUp ? "Operational" : "Down")
                    ]);

                var response = new AdminDashboardResponse(
                    start,
                    lastDay,
                    stats,
                    registrations,
                    distribution,
                    contentActivity,
                    recentRegistrations,
                    recentActivity,
                    topReports,
                    health);

                return Result<AdminDashboardResponse>.Success(response, "Admin dashboard retrieved successfully");
            }

            private static StatValue Stat(int total, int current, int previous) =>
                new(total, PercentChange(current, previous));

            private static decimal PercentChange(int current, int previous)
            {
                if (previous == 0) return current > 0 ? 100 : 0;
                return Math.Round((decimal)(current - previous) / previous * 100, 1);
            }
        }
    }

    public record StatValue(int Value, decimal ChangePercent);

    public record AdminDashboardStats(
        StatValue TotalUsers,
        StatValue ActiveUsers,
        StatValue Professionals,
        StatValue Recruiters,
        StatValue Companies,
        StatValue ActiveJobs,
        StatValue ActiveEvents,
        StatValue Posts,
        StatValue PendingReports,
        StatValue RestrictedAccounts);

    public record UserDistributionItem(string Role, int Count, decimal Percent);

    public record ServiceHealthDto(string Name, string Status);

    public record SystemHealthDto(string Overall, List<ServiceHealthDto> Services);

    public record AdminDashboardResponse(
        DateTime From,
        DateTime To,
        AdminDashboardStats Stats,
        List<DailyRegistrationsRow> RegistrationsByDay,
        List<UserDistributionItem> UserDistribution,
        List<DailyContentActivityRow> ContentActivityByDay,
        List<RecentRegistrationRow> RecentRegistrations,
        List<RecentActivityRow> RecentActivity,
        List<TopReportRow> TopReports,
        SystemHealthDto SystemHealth);
}