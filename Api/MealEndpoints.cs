using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class MealEndpoints
{
    public static WebApplication MapMealEndpoints(this WebApplication app)
    {
        var rGroup = app.MapGroup("/api/recipes");

        rGroup.MapGet("/", async (MealService svc) =>
            Results.Ok(await svc.GetRecipesAsync()));

        rGroup.MapGet("/{id:int}", async (int id, MealService svc) =>
        {
            var r = await svc.GetRecipeByIdAsync(id);
            return r is not null ? Results.Ok(r) : Results.NotFound(new { error = "Recipe not found" });
        });

        rGroup.MapPost("/", async (CreateRecipeRequest req, MealService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateRecipeAsync(req);
            return Results.Created($"/api/recipes/{created.Id}", created);
        });

        rGroup.MapPut("/{id:int}", async (int id, UpdateRecipeRequest req, MealService svc) =>
        {
            var updated = await svc.UpdateRecipeAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Recipe not found" });
        });

        rGroup.MapDelete("/{id:int}", async (int id, MealService svc) =>
        {
            var deleted = await svc.DeleteRecipeAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Recipe not found" });
        });

        rGroup.MapPatch("/{id:int}/favorite", async (int id, MealService svc) =>
        {
            var result = await svc.ToggleFavoriteAsync(id);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Recipe not found" });
        });

        var mGroup = app.MapGroup("/api/meals");

        mGroup.MapGet("/", async (string? week, MealService svc) =>
            Results.Ok(await svc.GetWeekPlanAsync(week)));

        mGroup.MapPut("/", async (SetMealSlotRequest req, MealService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Date) || string.IsNullOrWhiteSpace(req.Slot))
                return Results.BadRequest(new { error = "Date and Slot are required" });
            var result = await svc.SetMealSlotAsync(req);
            return Results.Ok(result);
        });

        mGroup.MapDelete("/{id:int}", async (int id, MealService svc) =>
        {
            var deleted = await svc.DeleteMealSlotAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Meal slot not found" });
        });

        mGroup.MapDelete("/week", async (string? week, MealService svc) =>
        {
            var count = await svc.ClearWeekAsync(week);
            return Results.Ok(new { cleared = count });
        });

        return app;
    }
}
