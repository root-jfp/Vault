using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class ProjectEndpoints
{
    public static WebApplication MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects");

        group.MapGet("/", async (ProjectService svc) =>
            Results.Ok(await svc.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, ProjectService svc) =>
        {
            var p = await svc.GetByIdAsync(id);
            return p is not null ? Results.Ok(p) : Results.NotFound(new { error = "Project not found" });
        });

        group.MapPost("/", async (CreateProjectRequest req, ProjectService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/projects/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateProjectRequest req, ProjectService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Project not found" });
        });

        group.MapDelete("/{id:int}", async (int id, ProjectService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Project not found" });
        });

        return app;
    }
}
