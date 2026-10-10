using Application.Common.Dtos;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Application.Queries.Admin
{
    public class GetAdminSystemHealth
    {
        public record GetAdminSystemHealthQuery() : IRequest<Result<AdminSystemHealthResponse>>;

        public class GetAdminSystemHealthHandler(HealthCheckService healthCheckService)
            : IRequestHandler<GetAdminSystemHealthQuery, Result<AdminSystemHealthResponse>>
        {
            public async Task<Result<AdminSystemHealthResponse>> Handle(
                GetAdminSystemHealthQuery request, CancellationToken cancellationToken)
            {
                var report = await healthCheckService.CheckHealthAsync(cancellationToken);

                var services = report.Entries
                    .Select(e => new AdminServiceHealthItem(
                        e.Key,
                        e.Value.Status.ToString(),
                        e.Value.Description,
                        e.Value.Duration.TotalMilliseconds))
                    .ToList();

                var response = new AdminSystemHealthResponse(
                    report.Status.ToString(),
                    DateTime.UtcNow,
                    services);

                return Result<AdminSystemHealthResponse>.Success(response, "System health retrieved successfully");
            }
        }
    }

    public record AdminServiceHealthItem(string Name, string Status, string? Description, double ResponseTimeMs);

    public record AdminSystemHealthResponse(string Overall, DateTime CheckedAt, List<AdminServiceHealthItem> Services);
}