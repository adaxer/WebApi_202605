using Microsoft.Extensions.Diagnostics.HealthChecks;
using MovieBase.Common.Data;

namespace MovieBase.Api;

public class DatabaseConnectedHealthCheck : IHealthCheck
{
    private readonly MoviesContext _moviesContext;
    private readonly ILogger<DatabaseConnectedHealthCheck> _logger;

    public DatabaseConnectedHealthCheck(MoviesContext moviesContext, ILogger<DatabaseConnectedHealthCheck> logger)
    {
        _moviesContext = moviesContext;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        try
        {
            isHealthy = _moviesContext.Database.CanConnect();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database not connectable");
            isHealthy = false;
        }

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("Database online"));
        }

        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, "Database not connectable"));
    }
}
