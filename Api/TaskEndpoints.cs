using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class TaskEndpoints
{
    public static WebApplication MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tasks");

        group.MapGet("/columns", async (TaskService svc) =>
            Results.Ok(await svc.GetColumnsAsync()));

        group.MapGet("/{id:int}", async (int id, TaskService svc) =>
        {
            var task = await svc.GetTaskByIdAsync(id);
            return task is not null
                ? Results.Ok(task)
                : Results.NotFound(new { error = "Task not found" });
        });

        group.MapPost("/", async (CreateTaskRequest req, TaskService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return Results.BadRequest(new { error = "Title is required" });
            if (req.Title.Length > 300)
                return Results.BadRequest(new { error = "Title must be 300 characters or fewer" });
            if (req.Description?.Length > 2000)
                return Results.BadRequest(new { error = "Description must be 2000 characters or fewer" });
            if (req.Project?.Length > 200)
                return Results.BadRequest(new { error = "Project must be 200 characters or fewer" });
            var created = await svc.CreateTaskAsync(req);
            return created is not null
                ? Results.Created($"/api/tasks/{created.Id}", created)
                : Results.BadRequest(new { error = "Column not found" });
        });

        group.MapPut("/{id:int}", async (int id, UpdateTaskRequest req, TaskService svc) =>
        {
            if (req.Title?.Length > 300)
                return Results.BadRequest(new { error = "Title must be 300 characters or fewer" });
            if (req.Description?.Length > 2000)
                return Results.BadRequest(new { error = "Description must be 2000 characters or fewer" });
            if (req.Project?.Length > 200)
                return Results.BadRequest(new { error = "Project must be 200 characters or fewer" });
            var updated = await svc.UpdateTaskAsync(id, req);
            return updated is not null
                ? Results.Ok(updated)
                : Results.NotFound(new { error = "Task not found" });
        });

        group.MapDelete("/{id:int}", async (int id, TaskService svc) =>
        {
            var deleted = await svc.DeleteTaskAsync(id);
            return deleted
                ? Results.Ok(new { success = true })
                : Results.NotFound(new { error = "Task not found" });
        });

        group.MapPatch("/{id:int}/move", async (int id, MoveTaskRequest req, TaskService svc) =>
        {
            if (req.Position < 0)
                return Results.BadRequest(new { error = "Position must be >= 0" });
            var result = await svc.MoveTaskAsync(id, req);
            return result is not null
                ? Results.Ok(result)
                : Results.NotFound(new { error = "Task or target column not found" });
        });

        return app;
    }
}
