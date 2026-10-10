namespace Application.Common.Repositories
{
    public interface IAdminDashboardRepository
    {
        Task<AdminDashboardTotals> GetTotalsAsync(CancellationToken ct = default);
        Task<AdminDashboardTotals> GetCreatedBetweenAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<List<DailyRegistrationsRow>> GetRegistrationsByDayAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<List<RoleCountRow>> GetUserDistributionAsync(CancellationToken ct = default);
        Task<List<DailyContentActivityRow>> GetContentActivityByDayAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<List<RecentRegistrationRow>> GetRecentRegistrationsAsync(int take, CancellationToken ct = default);
        Task<List<RecentActivityRow>> GetRecentActivityAsync(int take, CancellationToken ct = default);
        Task<List<TopReportRow>> GetTopReportsAsync(DateTime start, DateTime end, int take, CancellationToken ct = default);
        Task<bool> IsDatabaseReachableAsync(CancellationToken ct = default);

        Task<List<CategoryCountRow>> GetTopJobCategoriesAsync(int take, CancellationToken ct = default);
        Task<List<ReasonCountRow>> GetReportsByReasonAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<List<DailyPlatformActivityRow>> GetPlatformActivityByDayAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<List<TopCompanyActivityRow>> GetTopCompaniesByActivityAsync(DateTime start, DateTime end, int take, CancellationToken ct = default);
    }

    public record AdminDashboardTotals(
        int TotalUsers,
        int ActiveUsers,
        int Professionals,
        int Recruiters,
        int Companies,
        int ActiveJobs,
        int ActiveEvents,
        int Posts,
        int PendingReports,
        int RestrictedAccounts);

    public record DailyRegistrationsRow(DateTime Date, int Total, int Professionals, int Recruiters, int Companies);

    public record RoleCountRow(string Role, int Count);

    public record DailyContentActivityRow(DateTime Date, int Posts, int Comments, int Shares);

    public record RecentRegistrationRow(
        Guid UserId,
        string Name,
        string? AvatarUrl,
        string Type,
        DateTime JoinedAt,
        string Status);

    public record RecentActivityRow(string Action, string Description, DateTime OccurredAt);

    public record TopReportRow(string ContentType, string Reason, int Count);

    public record CategoryCountRow(string Category, int Count);
    public record ReasonCountRow(string Reason, int Count);
    public record DailyPlatformActivityRow(DateTime Date, int Posts, int Applications);
    public record TopCompanyActivityRow(Guid CompanyId, string CompanyName, string? LogoUrl, int JobsPosted, int Applications, decimal EngagementPercent);
}