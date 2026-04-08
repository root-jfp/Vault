using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class ShoppingEndpoints
{
    public static WebApplication MapShoppingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/shopping");

        group.MapGet("/", async (ShoppingService svc) =>
            Results.Ok(await svc.GetAllListsAsync()));

        group.MapGet("/default", async (ShoppingService svc) =>
        {
            var list = await svc.GetDefaultListAsync();
            return list is not null ? Results.Ok(list) : Results.NotFound(new { error = "No shopping list found" });
        });

        group.MapGet("/{listId:int}", async (int listId, ShoppingService svc) =>
        {
            var list = await svc.GetListAsync(listId);
            return list is not null ? Results.Ok(list) : Results.NotFound(new { error = "Shopping list not found" });
        });

        group.MapPost("/", async (CreateShoppingListRequest req, ShoppingService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateListAsync(req);
            return Results.Created($"/api/shopping/{created.Id}", created);
        });

        group.MapDelete("/{id:int}", async (int id, ShoppingService svc) =>
        {
            var deleted = await svc.DeleteListAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Shopping list not found" });
        });

        group.MapPost("/{listId:int}/items", async (int listId, CreateShoppingItemRequest req, ShoppingService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.AddItemAsync(listId, req);
            return created is not null
                ? Results.Created($"/api/shopping/items/{created.Id}", created)
                : Results.NotFound(new { error = "Shopping list not found" });
        });

        group.MapPut("/items/{id:int}", async (int id, UpdateShoppingItemRequest req, ShoppingService svc) =>
        {
            var updated = await svc.UpdateItemAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Item not found" });
        });

        group.MapDelete("/items/{id:int}", async (int id, ShoppingService svc) =>
        {
            var deleted = await svc.DeleteItemAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Item not found" });
        });

        group.MapPatch("/items/{id:int}/toggle", async (int id, ShoppingService svc) =>
        {
            var result = await svc.ToggleItemAsync(id);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Item not found" });
        });

        group.MapDelete("/{listId:int}/done", async (int listId, ShoppingService svc) =>
        {
            var count = await svc.ClearDoneItemsAsync(listId);
            return Results.Ok(new { cleared = count });
        });

        return app;
    }
}
