using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class HabitEndpoints
{
    public static WebApplication MapHabitEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/habits");

        group.MapGet("/", async (HabitService svc) =>
            Results.Ok(await svc.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, HabitService svc) =>
        {
            var habit = await svc.GetByIdAsync(id);
            return habit is not null ? Results.Ok(habit) : Results.NotFound(new { error = "Habit not found" });
        });

        group.MapPost("/", async (CreateHabitRequest req, HabitService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/habits/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateHabitRequest req, HabitService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Habit not found" });
        });

        group.MapDelete("/{id:int}", async (int id, HabitService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Habit not found" });
        });

        group.MapPost("/{id:int}/toggle", async (int id, ToggleHabitRequest req, HabitService svc) =>
        {
            var result = await svc.ToggleAsync(id, req);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Habit not found" });
        });

        group.MapPost("/{id:int}/archive", async (int id, HabitService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Habit not found" });
        });

        group.MapGet("/{id:int}/heatmap", async (int id, int year, int month, HabitService svc) =>
        {
            if (year < 2000 || year > 2100 || month < 1 || month > 12)
                return Results.BadRequest(new { error = "Invalid year or month" });
            return Results.Ok(await svc.GetHeatmapAsync(id, year, month));
        });

        return app;
    }
}
