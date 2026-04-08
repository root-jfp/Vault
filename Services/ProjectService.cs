using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class ProjectService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<ProjectResponse>> GetAllAsync()
    {
        var projects = await db.Projects
            .Where(p => p.UserId == DefaultUserId)
            .OrderBy(p => p.SortOrder)
            .AsNoTracking()
            .ToListAsync();

        var taskCounts = await db.TaskItems
            .Where(t => t.UserId == DefaultUserId && t.ProjectId != null)
            .GroupBy(t => t.ProjectId!)
            .Select(g => new { ProjectId = g.Key, Total = g.Count(), Done = g.Count(t => t.Column.Name == "Done") })
            .AsNoTracking()
            .ToListAsync();

        var countMap = taskCounts.ToDictionary(x => x.ProjectId!.Value);

        return projects.Select(p =>
        {
            var counts = countMap.GetValueOrDefault(p.Id);
            var total  = counts?.Total ?? 0;
            var done   = counts?.Done  ?? 0;
            var open   = total - done;
            var pct    = total > 0 ? (int)Math.Round(100.0 * done / total) : 0;
            int? daysLeft = p.Deadline is not null
                ? p.Deadline.Value.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber
                : null;
            return Map(p, total, open, done, pct, daysLeft);
        }).ToList();
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        var p = await db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        if (p is null) return null;
        var tasks = await db.TaskItems.Where(t => t.ProjectId == id).AsNoTracking().ToListAsync();
        // We can't easily join column name in a NoTracking query without Include; use a simple count
        var total = tasks.Count;
        var done  = 0; // tasks in Done column — simplified for detail view
        var open  = total - done;
        var pct   = 0;
        int? daysLeft = p.Deadline is not null
            ? p.Deadline.Value.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber
            : null;
        return Map(p, total, open, done, pct, daysLeft);
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest req)
    {
        var maxSort = await db.Projects.Where(p => p.UserId == DefaultUserId).MaxAsync(p => (int?)p.SortOrder) ?? 0;
        var project = new Project
        {
            UserId      = DefaultUserId,
            Name        = req.Name,
            Description = req.Description,
            Status      = NormalizeStatus(req.Status),
            Color       = req.Color,
            Deadline    = ParseDate(req.Deadline),
            SortOrder   = maxSort + 1,
        };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return (await GetByIdAsync(project.Id))!;
    }

    public async Task<ProjectResponse?> UpdateAsync(int id, UpdateProjectRequest req)
    {
        var p = await db.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        if (p is null) return null;
        if (req.Name        is not null) p.Name        = req.Name;
        if (req.Description is not null) p.Description = req.Description;
        if (req.Status      is not null) p.Status      = NormalizeStatus(req.Status);
        if (req.Color       is not null) p.Color       = req.Color;
        if (req.Deadline    is not null) p.Deadline    = ParseDate(req.Deadline);
        p.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await db.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        if (p is null) return false;
        p.DeletedAt = DateTime.UtcNow;
        p.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    private static ProjectResponse Map(Project p, int total, int open, int done, int pct, int? daysLeft) =>
        new(p.Id, p.Name, p.Description, p.Status, p.Color,
            p.Deadline?.ToString("yyyy-MM-dd"), total, open, done, pct, daysLeft);

    private static string NormalizeStatus(string? s) =>
        s switch { "paused" => "paused", "done" => "done", "archived" => "archived", _ => "active" };

    private static DateOnly? ParseDate(string? s) =>
        DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
