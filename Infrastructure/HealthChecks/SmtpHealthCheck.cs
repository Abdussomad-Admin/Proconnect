using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.HealthChecks
{
    public class SmtpHealthCheck(IConfiguration configuration) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var host = configuration["EmailSettings:Host"];
            var port = configuration.GetValue<int>("EmailSettings:Port");

            if (string.IsNullOrWhiteSpace(host))
                return HealthCheckResult.Unhealthy("SMTP host not configured");

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                return HealthCheckResult.Healthy("SMTP server reachable");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("SMTP server unreachable", ex);
            }
        }
    }
}