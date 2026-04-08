using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class CalendarEndpoints
{
    public static WebApplication MapCalendarEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/calendar");

        group.MapGet("/", async (string? from, string? to, CalendarService svc) =>
        {
            var today    = DateOnly.FromDateTime(DateTime.UtcNow);
            var fromDate = ParseDate(from) ?? today.AddDays(-today.Day + 1);
            var toDate   = ParseDate(to)   ?? fromDate.AddDays(41);
            return Results.Ok(await svc.GetEventsAsync(fromDate, toDate));
        });

        group.MapGet("/{id:int}", async (int id, CalendarService svc) =>
        {
            var e = await svc.GetByIdAsync(id);
            return e is not null ? Results.Ok(e) : Results.NotFound(new { error = "Event not found" });
        });

        group.MapPost("/", async (CreateCalendarEventRequest req, CalendarService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return Results.BadRequest(new { error = "Title is required" });
            if (string.IsNullOrWhiteSpace(req.Date))
                return Results.BadRequest(new { error = "Date is required" });
            var created = await svc.CreateAsync(req);
            return Results.Created($"/api/calendar/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateCalendarEventRequest req, CalendarService svc) =>
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated is not null ? Results.Ok(updated) : Results.NotFound(new { error = "Event not found" });
        });

        group.MapDelete("/{id:int}", async (int id, CalendarService svc) =>
        {
            var deleted = await svc.DeleteAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Event not found" });
        });

        return app;
    }

    private static DateOnly? ParseDate(string? s) =>
        DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
