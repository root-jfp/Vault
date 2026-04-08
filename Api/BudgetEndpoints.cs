using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class BudgetEndpoints
{
    public static WebApplication MapBudgetEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/budget");

        group.MapGet("/summary", async (string? month, BudgetService svc) =>
            Results.Ok(await svc.GetSummaryAsync(month)));

        group.MapGet("/categories", async (string? month, BudgetService svc) =>
            Results.Ok(await svc.GetCategoriesAsync(month)));

        group.MapPost("/categories", async (CreateCategoryRequest req, BudgetService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateCategoryAsync(req);
            return Results.Created($"/api/budget/categories/{created!.Id}", created);
        });

        group.MapPut("/categories/{id:int}", async (int id, UpdateCategoryRequest req, BudgetService svc) =>
        {
            var updated = await svc.UpdateCategoryAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Category not found" });
        });

        group.MapDelete("/categories/{id:int}", async (int id, BudgetService svc) =>
        {
            var deleted = await svc.DeleteCategoryAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Category not found" });
        });

        group.MapGet("/transactions", async (string? month, int? categoryId, BudgetService svc) =>
            Results.Ok(await svc.GetTransactionsAsync(month, categoryId)));

        group.MapPost("/transactions", async (CreateTransactionRequest req, BudgetService svc) =>
        {
            if (req.Amount <= 0)
                return Results.BadRequest(new { error = "Amount must be > 0" });
            var created = await svc.CreateTransactionAsync(req);
            return created is not null
                ? Results.Created($"/api/budget/transactions/{created.Id}", created)
                : Results.BadRequest(new { error = "Category not found" });
        });

        group.MapPut("/transactions/{id:int}", async (int id, UpdateTransactionRequest req, BudgetService svc) =>
        {
            var updated = await svc.UpdateTransactionAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Transaction not found" });
        });

        group.MapDelete("/transactions/{id:int}", async (int id, BudgetService svc) =>
        {
            var deleted = await svc.DeleteTransactionAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Transaction not found" });
        });

        return app;
    }
}
