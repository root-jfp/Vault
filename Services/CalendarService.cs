using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class CalendarService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<CalendarEventResponse>> GetEventsAsync(DateOnly from, DateOnly to)
    {
        var events = new List<CalendarEventResponse>();

        // 1. Manual calendar events
        var manual = await db.CalendarEvents
            .Where(e => e.UserId == DefaultUserId && e.Date >= from && e.Date <= to)
            .AsNoTracking().ToListAsync();
        events.AddRange(manual.Select(e => MapEvent(e, "manual")));

        // 2. Tasks with due dates in range
        var tasks = await db.TaskItems
            .Where(t => t.UserId == DefaultUserId && t.DueDate >= from && t.DueDate <= to)
            .AsNoTracking().ToListAsync();
        events.AddRange(tasks.Select(t => new CalendarEventResponse(
            t.Id, t.Title, t.DueDate!.Value.ToString("yyyy-MM-dd"), null, null,
            t.Priority == "high" ? "#E24B4A" : t.Priority == "med" ? "#EF9F27" : "#6e6d68",
            t.Description, "task", t.Id, false, "task")));

        // 3. Maintenance items due in range
        var maintenance = await db.MaintenanceItems
            .Where(m => m.UserId == DefaultUserId && m.NextDueAt != null)
            .AsNoTracking().ToListAsync();
        foreach (var m in maintenance)
        {
            var dueDate = DateOnly.FromDateTime(m.NextDueAt!.Value);
            if (dueDate >= from && dueDate <= to)
                events.Add(new CalendarEventResponse(
                    m.Id, $"🔧 {m.Name}", dueDate.ToString("yyyy-MM-dd"), null, null,
                    "#EF9F27", m.Notes, "maintenance", m.Id, false, "maintenance"));
        }

        // 4. Birthdays/anniversaries in range (recurring yearly)
        var persons = await db.Persons.Where(p => p.UserId == DefaultUserId).AsNoTracking().ToListAsync();
        foreach (var p in persons)
        {
            for (int yr = from.Year; yr <= to.Year; yr++)
            {
                try
                {
                    var occ = new DateOnly(yr, p.Date.Month, p.Date.Day);
                    if (occ >= from && occ <= to)
                    {
                        var age = yr - p.Date.Year;
                        var label = p.EventType == "anniversary" ? $"💑 {p.Name}" : $"🎂 {p.Name} ({age})";
                        events.Add(new CalendarEventResponse(
                            p.Id, label, occ.ToString("yyyy-MM-dd"), null, null,
                            "#C77DD3", null, "birthday", p.Id, true, "birthday"));
                    }
                }
                catch { /* Feb 29 in non-leap years */ }
            }
        }

        return events.OrderBy(e => e.Date).ToList();
    }

    public async Task<CalendarEventResponse?> GetByIdAsync(int id)
    {
        var e = await db.CalendarEvents.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && e.UserId == DefaultUserId);
        return e is null ? null : MapEvent(e, "manual");
    }

    public async Task<CalendarEventResponse> CreateAsync(CreateCalendarEventRequest req)
    {
        var date = ParseDate(req.Date) ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var e = new CalendarEvent
        {
            UserId         = DefaultUserId,
            Title          = req.Title,
            Date           = date,
            StartTime      = req.StartTime is not null && TimeOnly.TryParse(req.StartTime, out var st) ? st : null,
            EndTime        = req.EndTime   is not null && TimeOnly.TryParse(req.EndTime,   out var et) ? et : null,
            Color          = req.Color,
            Description    = req.Description,
            LinkedModule   = "manual",
            IsRecurring    = req.IsRecurring ?? false,
            RecurrenceRule = req.RecurrenceRule,
        };
        db.CalendarEvents.Add(e);
        await db.SaveChangesAsync();
        return MapEvent(e, "manual");
    }

    public async Task<CalendarEventResponse?> UpdateAsync(int id, UpdateCalendarEventRequest req)
    {
        var e = await db.CalendarEvents.FirstOrDefaultAsync(e => e.Id == id && e.UserId == DefaultUserId);
        if (e is null) return null;
        if (req.Title       is not null) e.Title       = req.Title;
        if (req.Date        is not null) e.Date        = ParseDate(req.Date) ?? e.Date;
        if (req.Color       is not null) e.Color       = req.Color;
        if (req.Description is not null) e.Description = req.Description;
        if (req.StartTime   is not null) e.StartTime   = TimeOnly.TryParse(req.StartTime, out var st) ? st : e.StartTime;
        if (req.EndTime     is not null) e.EndTime     = TimeOnly.TryParse(req.EndTime,   out var et) ? et : e.EndTime;
        e.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapEvent(e, "manual");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await db.CalendarEvents.FirstOrDefaultAsync(e => e.Id == id && e.UserId == DefaultUserId);
        if (e is null) return false;
        e.DeletedAt = DateTime.UtcNow;
        e.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    private static CalendarEventResponse MapEvent(CalendarEvent e, string source) =>
        new(e.Id, e.Title, e.Date.ToString("yyyy-MM-dd"),
            e.StartTime?.ToString("HH:mm"), e.EndTime?.ToString("HH:mm"),
            e.Color, e.Description, e.LinkedModule, e.LinkedId, e.IsRecurring, source);

    private static DateOnly? ParseDate(string? s) =>
        DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
