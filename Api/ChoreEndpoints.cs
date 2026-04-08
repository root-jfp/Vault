using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class ChoreEndpoints
{
    public static WebApplication MapChoreEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/chores");

        group.MapGet("/", async (string? week, ChoreService svc) =>
            Results.Ok(await svc.GetWeekAsync(week)));

        group.MapGet("/{id:int}", async (int id, ChoreService svc) =>
        {
            var chore = await svc.GetByIdAsync(id);
            return chore is not null ? Results.Ok(chore) : Results.NotFound(new { error = "Chore not found" });
        });

        group.MapPost("/", async (CreateChoreRequest req, ChoreService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateAsync(req);
            return created is not null
                ? Results.Created($"/api/chores/{created.Id}", created)
                : Results.StatusCode(500);
        });

        group.MapPut("/{id:int}", async (int id, UpdateChoreRequest req, ChoreService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Chore not found" });
        });

        group.MapDelete("/{id:int}", async (int id, ChoreService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Chore not found" });
        });

        group.MapPost("/{id:int}/complete", async (int id, CompleteChoreRequest req, ChoreService svc) =>
        {
            var result = await svc.CompleteAsync(id, req);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Chore not found" });
        });

        group.MapPatch("/{id:int}/swap", async (int id, SwapChoreRequest req, ChoreService svc) =>
        {
            var result = await svc.SwapAsync(id, req);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Chore not found" });
        });

        return app;
    }
}
