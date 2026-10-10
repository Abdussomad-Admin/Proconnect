using Application.Common.Repositories;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class AdminDashboardRepository(ProConnectDbContext db) : IAdminDashboardRepository
    {
        public Task<AdminDashboardTotals> GetTotalsAsync(CancellationToken ct = default) =>
            BuildTotalsAsync(null, null, ct);

        public Task<AdminDashboardTotals> GetCreatedBetweenAsync(DateTime start, DateTime end, CancellationToken ct = default) =>
            BuildTotalsAsync(start, end, ct);

        private async Task<AdminDashboardTotals> BuildTotalsAsync(DateTime? start, DateTime? end, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            var users = db.Users.Where(u => !u.IsDeleted && u.Role.ToLower() != "admin"
                && (start == null || u.DateCreated >= start) && (end == null || u.DateCreated < end));

            var companies = db.Companies.Where(c => !c.IsDeleted
                && (start == null || c.DateCreated >= start) && (end == null || c.DateCreated < end));

            var jobs = db.Jobs.Where(j => j.IsActive
                && (start == null || j.DateCreated >= start) && (end == null || j.DateCreated < end));

            var events = db.Events.Where(e => !e.IsDeleted && e.Status == EventStatus.Published && e.EndDateTime >= now
                && (start == null || e.DateCreated >= start) && (end == null || e.DateCreated < end));

            var posts = db.Posts.Where(p => !p.IsDeleted && p.OriginalPostId == null
                && (start == null || p.DateCreated >= start) && (end == null || p.DateCreated < end));

            var reports = db.Reports.Where(r => r.ReportStatus == ReportStatus.Pending
                && (start == null || r.DateCreated >= start) && (end == null || r.DateCreated < end));

            return new AdminDashboardTotals(
                TotalUsers: await users.CountAsync(ct),
                ActiveUsers: await users.CountAsync(u => u.Status == UserStatus.Active, ct),
                Professionals: await users.CountAsync(u => u.ProfessionalProfile != null, ct),
                Recruiters: await users.CountAsync(u => u.RecruiterProfile != null, ct),
                Companies: await companies.CountAsync(ct),
                ActiveJobs: await jobs.CountAsync(ct),
                ActiveEvents: await events.CountAsync(ct),
                Posts: await posts.CountAsync(ct),
                PendingReports: await reports.CountAsync(ct),
                RestrictedAccounts: await users.CountAsync(u => u.Status == UserStatus.Suspended, ct));
        }

        public async Task<List<CategoryCountRow>> GetTopJobCategoriesAsync(int take, CancellationToken ct = default)
        {
            return await db.Jobs
                .Where(j => j.IsActive)
                .GroupBy(j => j.Department.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .Select(x => new CategoryCountRow(x.Category, x.Count))
                .ToListAsync(ct);
        }

        public async Task<List<ReasonCountRow>> GetReportsByReasonAsync(DateTime start, DateTime end, CancellationToken ct = default)
        {
            var rows = await db.Reports
                .Where(r => r.DateCreated >= start && r.DateCreated < end)
                .GroupBy(r => r.Reason)
                .Select(g => new { Reason = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync(ct);

            return rows.Select(x => new ReasonCountRow(x.Reason.ToString(), x.Count)).ToList();
        }

        public async Task<List<DailyPlatformActivityRow>> GetPlatformActivityByDayAsync(DateTime start, DateTime end, CancellationToken ct = default)
        {
            var posts = await db.Posts
                .Where(p => !p.IsDeleted && p.OriginalPostId == null && p.DateCreated >= start && p.DateCreated < end)
                .GroupBy(p => p.DateCreated.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var applications = await db.JobApplications
                .Where(a => a.AppliedAt >= start && a.AppliedAt < end)
                .GroupBy(a => a.AppliedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var allDates = posts.Select(x => x.Date).Union(applications.Select(x => x.Date)).Distinct().OrderBy(d => d);

            return allDates
                .Select(d => new DailyPlatformActivityRow(
                    d,
                    posts.FirstOrDefault(x => x.Date == d)?.Count ?? 0,
                    applications.FirstOrDefault(x => x.Date == d)?.Count ?? 0))
                .ToList();
        }

        public async Task<List<TopCompanyActivityRow>> GetTopCompaniesByActivityAsync(DateTime start, DateTime end, int take, CancellationToken ct = default)
        {
            var rows = await db.Companies
                .Where(c => !c.IsDeleted)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.LogoUrl,
                    JobsPosted = c.Jobs.Count(j => j.DateCreated >= start && j.DateCreated < end),
                    Applications = c.Jobs.SelectMany(j => j.JobApplications).Count(a => a.AppliedAt >= start && a.AppliedAt < end)
                })
                .Where(x => x.JobsPosted > 0)
                .OrderByDescending(x => x.Applications)
                .Take(take)
                .ToListAsync(ct);

            return rows
                .Select(x => new TopCompanyActivityRow(
                    x.Id, x.Name, x.LogoUrl, x.JobsPosted, x.Applications,
                    x.JobsPosted == 0 ? 0 : Math.Round((decimal)x.Applications / x.JobsPosted * 100, 1)))
                .ToList();
        }

        public async Task<List<DailyRegistrationsRow>> GetRegistrationsByDayAsync(
            DateTime start, DateTime end, CancellationToken ct = default)
        {
            var users = await db.Users
                .Where(u => !u.IsDeleted && u.Role.ToLower() != "admin" && u.DateCreated >= start && u.DateCreated < end)
                .Select(u => new
                {
                    u.DateCreated,
                    IsProfessional = u.ProfessionalProfile != null,
                    IsRecruiter = u.RecruiterProfile != null
                })
                .ToListAsync(ct);

            var companyDates = await db.Companies
                .Where(c => !c.IsDeleted && c.DateCreated >= start && c.DateCreated < end)
                .Select(c => c.DateCreated)
                .ToListAsync(ct);

            var companiesByDay = companyDates.GroupBy(d => d.Date).ToDictionary(g => g.Key, g => g.Count());

            return users
                .GroupBy(u => u.DateCreated.Date)
                .Select(g => new DailyRegistrationsRow(
                    g.Key,
                    Total: g.Count(),
                    Professionals: g.Count(u => u.IsProfessional),
                    Recruiters: g.Count(u => u.IsRecruiter),
                    Companies: companiesByDay.GetValueOrDefault(g.Key)))
                .ToList();
        }

        public async Task<List<RoleCountRow>> GetUserDistributionAsync(CancellationToken ct = default)
        {
            var users = db.Users.Where(u => !u.IsDeleted && u.Role.ToLower() != "admin");

            var professionals = await users.CountAsync(u => u.ProfessionalProfile != null, ct);
            var recruiters = await users.CountAsync(u => u.RecruiterProfile != null, ct);

            return
            [
                new RoleCountRow("Professionals", professionals),
                new RoleCountRow("Recruiters", recruiters)
            ];
        }

        public async Task<List<DailyContentActivityRow>> GetContentActivityByDayAsync(
            DateTime start, DateTime end, CancellationToken ct = default)
        {
            var posts = await db.Posts
                .Where(p => !p.IsDeleted && p.OriginalPostId == null && p.DateCreated >= start && p.DateCreated < end)
                .GroupBy(p => p.DateCreated.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var comments = await db.Comments
                .Where(c => c.DateCreated >= start && c.DateCreated < end)
                .GroupBy(c => c.DateCreated.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var shares = await db.Posts
                .Where(p => !p.IsDeleted && p.OriginalPostId != null && p.DateCreated >= start && p.DateCreated < end)
                .GroupBy(p => p.DateCreated.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var allDates = posts.Select(x => x.Date)
                .Union(comments.Select(x => x.Date))
                .Union(shares.Select(x => x.Date))
                .Distinct()
                .OrderBy(d => d);

            return allDates
                .Select(d => new DailyContentActivityRow(
                    d,
                    posts.FirstOrDefault(x => x.Date == d)?.Count ?? 0,
                    comments.FirstOrDefault(x => x.Date == d)?.Count ?? 0,
                    shares.FirstOrDefault(x => x.Date == d)?.Count ?? 0))
                .ToList();
        }

        public async Task<List<RecentRegistrationRow>> GetRecentRegistrationsAsync(int take, CancellationToken ct = default)
        {
            return await db.Users
                .Where(u => !u.IsDeleted && u.Role.ToLower() != "admin")
                .OrderByDescending(u => u.DateCreated)
                .Take(take)
                .Select(u => new RecentRegistrationRow(
                    u.Id,
                    u.FirstName + " " + u.LastName,
                    u.ProfilePictureUrl,
                    u.ProfessionalProfile != null ? "Professional"
                        : u.RecruiterProfile != null ? "Recruiter" : "User",
                    u.DateCreated,
                    u.Status.ToString()))
                .ToListAsync(ct);
        }

        public async Task<List<RecentActivityRow>> GetRecentActivityAsync(int take, CancellationToken ct = default)
        {
            var rows = await db.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(take)
                .Select(a => new { a.Action, a.Description, a.Timestamp })
                .ToListAsync(ct);

            return rows
                .Select(a => new RecentActivityRow(a.Action.ToString(), a.Description, a.Timestamp))
                .ToList();
        }

        public async Task<List<TopReportRow>> GetTopReportsAsync(
            DateTime start, DateTime end, int take, CancellationToken ct = default)
        {
            var rows = await db.Reports
                .Where(r => r.DateCreated >= start && r.DateCreated < end)
                .GroupBy(r => new { r.ContentType, r.Reason })
                .Select(g => new { g.Key.ContentType, g.Key.Reason, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync(ct);

            return rows
                .Select(x => new TopReportRow(x.ContentType.ToString(), x.Reason.ToString(), x.Count))
                .ToList();
        }

        public Task<bool> IsDatabaseReachableAsync(CancellationToken ct = default) =>
            db.Database.CanConnectAsync(ct);
    }
}