using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WorkshopDemo.HealthChecks;

public class WorkshopDemoHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
    }
}