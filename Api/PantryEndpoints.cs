using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class PantryEndpoints
{
    public static WebApplication MapPantryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/pantry");

        group.MapGet("/", async (PantryService svc) =>
            Results.Ok(await svc.GetAllAsync()));

        group.MapGet("/stats", async (PantryService svc) =>
            Results.Ok(await svc.GetStatsAsync()));

        group.MapGet("/{id:int}", async (int id, PantryService svc) =>
        {
            var item = await svc.GetByIdAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound(new { error = "Item not found" });
        });

        group.MapPost("/", async (CreatePantryItemRequest req, PantryService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/pantry/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdatePantryItemRequest req, PantryService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Item not found" });
        });

        group.MapDelete("/{id:int}", async (int id, PantryService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Item not found" });
        });

        group.MapPatch("/{id:int}/adjust", async (int id, AdjustPantryRequest req, PantryService svc) =>
        {
            var result = await svc.AdjustAsync(id, req);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Item not found" });
        });

        return app;
    }
}
