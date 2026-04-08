using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class NoteEndpoints
{
    public static WebApplication MapNoteEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notes");

        group.MapGet("/", async (string? search, NoteService svc) =>
            Results.Ok(await svc.GetAllAsync(search)));

        group.MapGet("/{id:int}", async (int id, NoteService svc) =>
        {
            var note = await svc.GetByIdAsync(id);
            return note is not null ? Results.Ok(note) : Results.NotFound(new { error = "Note not found" });
        });

        group.MapPost("/", async (CreateNoteRequest req, NoteService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return Results.BadRequest(new { error = "Title is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/notes/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateNoteRequest req, NoteService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Note not found" });
        });

        group.MapDelete("/{id:int}", async (int id, NoteService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Note not found" });
        });

        group.MapPatch("/{id:int}/pin", async (int id, NoteService svc) =>
        {
            var result = await svc.TogglePinAsync(id);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Note not found" });
        });

        group.MapPatch("/{id:int}/star", async (int id, NoteService svc) =>
        {
            var result = await svc.ToggleStarAsync(id);
            return result is not null ? Results.Ok(result) : Results.NotFound(new { error = "Note not found" });
        });

        return app;
    }
}
