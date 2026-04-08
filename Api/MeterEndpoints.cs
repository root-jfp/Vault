using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class MeterEndpoints
{
    public static WebApplication MapMeterEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/meters");

        group.MapGet("/", async (MeterService svc) =>
            Results.Ok(await svc.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, int? months, MeterService svc) =>
        {
            var result = await svc.GetByIdAsync(id, months ?? 6);
            return result is not null ? Results.Ok(new { meter = result.Value.meter, readings = result.Value.readings }) : Results.NotFound(new { error = "Meter not found" });
        });

        group.MapPost("/", async (CreateMeterRequest req, MeterService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/meters/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateMeterRequest req, MeterService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Meter not found" });
        });

        group.MapDelete("/{id:int}", async (int id, MeterService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Meter not found" });
        });

        group.MapPost("/{id:int}/readings", async (int id, LogReadingRequest req, MeterService svc) =>
        {
            var result = await svc.LogReadingAsync(id, req);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Meter not found" });
        });

        return app;
    }
}
