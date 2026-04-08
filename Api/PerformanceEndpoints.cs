using Vault.Services;

namespace Vault.Api;

public static class PerformanceEndpoints
{
    public static WebApplication MapPerformanceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/performance");

        group.MapGet("/summary", async (string? period, PerformanceService svc) =>
            Results.Ok(await svc.GetSummaryAsync(period ?? "week")));

        return app;
    }
}
