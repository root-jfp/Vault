using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class UserEndpoints
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/", async (UserService svc) =>
            Results.Ok(await svc.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, UserService svc) =>
        {
            var user = await svc.GetByIdAsync(id);
            return user is not null ? Results.Ok(user) : Results.NotFound(new { error = "User not found" });
        });

        group.MapPost("/", async (CreateUserRequest req, UserService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/users/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateUserRequest req, UserService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "User not found" });
        });

        return app;
    }
}
