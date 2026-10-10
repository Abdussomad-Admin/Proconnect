using CloudinaryDotNet;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.HealthChecks
{
    public class CloudinaryHealthCheck(Cloudinary cloudinary) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await cloudinary.GetUsageAsync();

                return result.StatusCode == System.Net.HttpStatusCode.OK
                    ? HealthCheckResult.Healthy("Cloudinary reachable")
                    : HealthCheckResult.Degraded($"Cloudinary returned {result.StatusCode}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Cloudinary unreachable", ex);
            }
        }
    }
}