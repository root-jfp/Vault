using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class TemplateService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<TemplateResponse>> GetAllAsync(string? category = null)
    {
        var query = db.Templates.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(t => t.Category == category);
        var templates = await query.OrderBy(t => t.Category).ThenBy(t => t.Name).AsNoTracking().ToListAsync();
        return templates.Select(Map).ToList();
    }

    public async Task<TemplateResponse?> GetByIdAsync(int id)
    {
        var t = await db.Templates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return t is null ? null : Map(t);
    }

    public async Task<object> ApplyAsync(int id)
    {
        var template = await db.Templates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (template is null) return new { error = "Template not found" };

        if (string.IsNullOrWhiteSpace(template.Payload))
            return new { error = "Template payload is empty" };

        JsonElement payload;
        try
        {
            payload = JsonSerializer.Deserialize<JsonElement>(template.Payload);
        }
        catch (JsonException ex)
        {
            return new { error = $"Template payload is invalid JSON: {ex.Message}" };
        }
        catch (ArgumentNullException ex)
        {
            return new { error = $"Template payload argument error: {ex.Message}" };
        }

        var created = new List<object>();

        if (template.Category == "habits" && payload.TryGetProperty("habits", out var habitsEl))
        {
            foreach (var h in habitsEl.EnumerateArray())
            {
                var maxSort = await db.Habits.Where(x => x.UserId == DefaultUserId).MaxAsync(x => (int?)x.SortOrder) ?? 0;
                var habit = new Habit
                {
                    UserId      = DefaultUserId,
                    Name        = h.TryGetProperty("name", out var n) ? n.GetString()! : "Habit",
                    Type        = h.TryGetProperty("type", out var tp) ? tp.GetString()! : "boolean",
                    Frequency   = h.TryGetProperty("freq", out var f) ? f.GetString()! : "Daily",
                    Color       = "var(--ac)",
                    HexColor    = "#1D9E75",
                    WeeklyGoal  = h.TryGetProperty("weeklyGoal", out var wg) ? wg.GetInt32() : 7,
                    Goal        = h.TryGetProperty("goal", out var g) ? g.GetDouble() : 0,
                    UnitOfMeasure = h.TryGetProperty("uom", out var u) ? u.GetString() : null,
                    SortOrder   = maxSort + 1,
                    DaysOfWeek  = "0,1,2,3,4,5,6",
                };
                db.Habits.Add(habit);
                created.Add(new { type = "habit", name = habit.Name });
            }
        }
        else if (template.Category is "tasks" or "projects" && payload.TryGetProperty("tasks", out var tasksEl))
        {
            var firstCol = await db.TaskColumns.Where(c => c.UserId == DefaultUserId).OrderBy(c => c.Position).FirstOrDefaultAsync();
            if (firstCol is not null)
            {
                foreach (var t in tasksEl.EnumerateArray())
                {
                    var maxPos = await db.TaskItems.Where(x => x.ColumnId == firstCol.Id).MaxAsync(x => (int?)x.Position) ?? -1;
                    var task = new TaskItem
                    {
                        UserId   = DefaultUserId,
                        ColumnId = firstCol.Id,
                        Title    = t.TryGetProperty("title", out var tl) ? tl.GetString()! : "Task",
                        Priority = t.TryGetProperty("priority", out var p) ? p.GetString()! : "med",
                        Position = maxPos + 1,
                    };
                    db.TaskItems.Add(task);
                    created.Add(new { type = "task", title = task.Title });
                }
            }
        }

        await db.SaveChangesAsync();
        return new { applied = id, created };
    }

    public async Task<TemplateResponse> CreateAsync(CreateTemplateRequest req)
    {
        var template = new Template
        {
            Name        = req.Name,
            Description = req.Description,
            Category    = req.Category,
            Icon        = req.Icon,
            Payload     = req.Payload,
            IsBuiltIn   = false,
        };
        db.Templates.Add(template);
        await db.SaveChangesAsync();
        return Map(template);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var t = await db.Templates.FirstOrDefaultAsync(t => t.Id == id && !t.IsBuiltIn);
        if (t is null) return false;
        db.Templates.Remove(t);
        await db.SaveChangesAsync();
        return true;
    }

    private static TemplateResponse Map(Template t) =>
        new(t.Id, t.Name, t.Description, t.Category, t.Icon, t.IsBuiltIn, t.Payload);
}
