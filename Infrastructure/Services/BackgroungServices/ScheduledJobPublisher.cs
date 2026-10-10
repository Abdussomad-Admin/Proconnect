using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices
{
    public class ScheduledJobPublisher(
        IServiceScopeFactory scopeFactory,
        ILogger<ScheduledJobPublisher> logger) : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PublishDueJobsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed while publishing scheduled jobs");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task PublishDueJobsAsync(CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProConnectDbContext>();

            var now = DateTime.UtcNow;

            var dueJobs = await db.Jobs
                .Where(j => j.Status == JobPostingStatus.Scheduled
                    && j.ScheduledPublishAt != null
                    && j.ScheduledPublishAt <= now)
                .ToListAsync(ct);

            if (dueJobs.Count == 0)
                return;

            foreach (var job in dueJobs)
            {
                job.Status = JobPostingStatus.PendingApproval;
                job.IsActive = false;
                job.ScheduledPublishAt = null;
                job.UpdatedAt = now;
            }

            await db.SaveChangesAsync(ct);

            logger.LogInformation("Moved {Count} scheduled job(s) to PendingApproval", dueJobs.Count);
        }
    }
}