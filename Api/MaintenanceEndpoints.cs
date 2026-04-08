using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class MaintenanceEndpoints
{
    public static WebApplication MapMaintenanceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/maintenance");

        group.MapGet("/", async (MaintenanceService svc) =>
            Results.Ok(await svc.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, MaintenanceService svc) =>
        {
            var item = await svc.GetByIdAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound(new { error = "Maintenance item not found" });
        });

        group.MapPost("/", async (CreateMaintenanceItemRequest req, MaintenanceService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            if (req.IntervalDays <= 0)
                return Results.BadRequest(new { error = "IntervalDays must be > 0" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/maintenance/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateMaintenanceItemRequest req, MaintenanceService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Maintenance item not found" });
        });

        group.MapDelete("/{id:int}", async (int id, MaintenanceService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Maintenance item not found" });
        });

        group.MapPost("/{id:int}/complete", async (int id, CompleteMaintenanceRequest req, MaintenanceService svc) =>
        {
            var result = await svc.CompleteAsync(id, req);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Maintenance item not found" });
        });

        return app;
    }
}
