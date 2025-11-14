using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ICTAce.FileHub.Features.Health;

/// <summary>
/// Health check for MyModules feature
/// </summary>
public class MyModulesHealthCheck : IHealthCheck
{
    private readonly IDbContextFactory<Context> _contextFactory;

    public MyModulesHealthCheck(IDbContextFactory<Context> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var db = _contextFactory.CreateDbContext();
            
            // Check if we can query the database
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy("Cannot connect to database");
            }

            // Check if MyModule table exists and is accessible
            var count = await db.MyModule.CountAsync(cancellationToken);
            
            return HealthCheckResult.Healthy($"MyModules feature is healthy. {count} modules in database.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"MyModules feature is unhealthy: {ex.Message}");
        }
    }
}
