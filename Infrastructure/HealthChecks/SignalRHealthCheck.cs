using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.HealthChecks
{
    public class SignalRHealthCheck<THub>(IHubContext<THub> hubContext) : IHealthCheck where THub : Hub
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                hubContext is not null
                    ? HealthCheckResult.Healthy("SignalR hub registered")
                    : HealthCheckResult.Unhealthy("SignalR hub not available"));
        }
    }
}