using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class HealthEndpoints
{
    public static WebApplication MapHealthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/health");

        group.MapGet("/", async (string type, int? days, HealthService svc) =>
            Results.Ok(await svc.GetMetricsAsync(type, days ?? 7)));

        group.MapGet("/summary", async (string type, HealthService svc) =>
            Results.Ok(await svc.GetSummaryAsync(type)));

        group.MapPost("/", async (LogHealthRequest req, HealthService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.MetricType))
                return Results.BadRequest(new { error = "MetricType is required" });
            var created = await svc.LogAsync(req);
            return Results.Ok(created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateHealthRequest req, HealthService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Metric not found" });
        });

        group.MapDelete("/{id:int}", async (int id, HealthService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Metric not found" });
        });

        group.MapPost("/water/add", async (QuickAddWaterRequest req, HealthService svc) =>
        {
            if (req.Amount <= 0)
                return Results.BadRequest(new { error = "Amount must be > 0" });
            var result = await svc.QuickAddWaterAsync(req.Amount);
            return Results.Ok(result);
        });

        return app;
    }
}
