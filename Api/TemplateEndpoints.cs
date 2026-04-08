using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class TemplateEndpoints
{
    public static WebApplication MapTemplateEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/templates");

        group.MapGet("/", async (string? category, TemplateService svc) =>
            Results.Ok(await svc.GetAllAsync(category)));

        group.MapGet("/{id:int}", async (int id, TemplateService svc) =>
        {
            var t = await svc.GetByIdAsync(id);
            return t is not null ? Results.Ok(t) : Results.NotFound(new { error = "Template not found" });
        });

        group.MapPost("/{id:int}/apply", async (int id, TemplateService svc) =>
            Results.Ok(await svc.ApplyAsync(id)));

        group.MapPost("/", async (CreateTemplateRequest req, TemplateService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/templates/{created.Id}", created);
        });

        group.MapDelete("/{id:int}", async (int id, TemplateService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Template not found or is built-in" });
        });

        return app;
    }
}
