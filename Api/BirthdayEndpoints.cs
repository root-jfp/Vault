using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class BirthdayEndpoints
{
    public static WebApplication MapBirthdayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/birthdays");

        group.MapGet("/", async (BirthdayService svc) =>
            Results.Ok(await svc.GetAllAsync()));

        group.MapGet("/upcoming", async (int? days, BirthdayService svc) =>
            Results.Ok(await svc.GetUpcomingAsync(days ?? 30)));

        group.MapGet("/{id:int}", async (int id, BirthdayService svc) =>
        {
            var p = await svc.GetByIdAsync(id);
            return p is not null ? Results.Ok(p) : Results.NotFound(new { error = "Person not found" });
        });

        group.MapPost("/", async (CreatePersonRequest req, BirthdayService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "Name is required" });
            if (string.IsNullOrWhiteSpace(req.Date))
                return Results.BadRequest(new { error = "Date is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/birthdays/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdatePersonRequest req, BirthdayService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Person not found" });
        });

        group.MapDelete("/{id:int}", async (int id, BirthdayService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Person not found" });
        });

        return app;
    }
}
