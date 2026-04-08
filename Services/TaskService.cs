using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class TaskService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<TaskColumnResponse>> GetColumnsAsync()
    {
        var columns = await db.TaskColumns
            .Where(c => c.UserId == DefaultUserId)
            .OrderBy(c => c.Position)
            .Include(c => c.Tasks.OrderBy(t => t.Position))
            .AsNoTracking()
            .ToListAsync();

        return columns.Select(c => MapColumnResponse(c)).ToList();
    }

    public async Task<TaskDetailResponse?> GetTaskByIdAsync(int id)
    {
        var task = await db.TaskItems
            .Include(t => t.Column)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == DefaultUserId);

        return task is null ? null : MapDetailResponse(task);
    }

    // Returns null when no valid target column can be resolved.
    public async Task<TaskDetailResponse?> CreateTaskAsync(CreateTaskRequest req)
    {
        int columnId;
        if (req.ColumnId is not null)
        {
            // Validate provided column belongs to this user
            var colExists = await db.TaskColumns
                .AnyAsync(c => c.Id == req.ColumnId && c.UserId == DefaultUserId);
            if (!colExists) return null;
            columnId = req.ColumnId.Value;
        }
        else
        {
            var firstCol = await db.TaskColumns
                .Where(c => c.UserId == DefaultUserId)
                .OrderBy(c => c.Position)
                .FirstOrDefaultAsync();
            if (firstCol is null) return null;
            columnId = firstCol.Id;
        }

        var maxPos = await db.TaskItems
            .Where(t => t.ColumnId == columnId)
            .MaxAsync(t => (int?)t.Position) ?? -1;

        var priority = NormalizePriority(req.Priority);

        var task = new TaskItem
        {
            UserId = DefaultUserId,
            ColumnId = columnId,
            Title = req.Title,
            Description = req.Description,
            Priority = priority,
            Project = req.Project,
            DueDate = ParseDate(req.Due),
            Position = maxPos + 1,
        };

        db.TaskItems.Add(task);
        await db.SaveChangesAsync();

        return await GetTaskByIdAsync(task.Id);
    }

    public async Task<TaskDetailResponse?> UpdateTaskAsync(int id, UpdateTaskRequest req)
    {
        var task = await db.TaskItems
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == DefaultUserId);

        if (task is null) return null;

        if (req.Title is not null) task.Title = req.Title;
        if (req.Description is not null) task.Description = req.Description == "" ? null : req.Description;
        if (req.Priority is not null) task.Priority = NormalizePriority(req.Priority);
        if (req.Project is not null) task.Project = req.Project == "" ? null : req.Project;
        if (req.Due is not null)
            task.DueDate = req.Due == "" ? null : ParseDate(req.Due);
        if (req.ColumnId is not null && req.ColumnId != task.ColumnId)
        {
            // Validate target column belongs to this user
            var colExists = await db.TaskColumns
                .AnyAsync(c => c.Id == req.ColumnId && c.UserId == DefaultUserId);
            if (!colExists) return null;
            var maxPos = await db.TaskItems
                .Where(t => t.ColumnId == req.ColumnId)
                .MaxAsync(t => (int?)t.Position) ?? -1;
            task.ColumnId = req.ColumnId.Value;
            task.Position = maxPos + 1;
        }
        task.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return (await GetTaskByIdAsync(id))!;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await db.TaskItems
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == DefaultUserId);

        if (task is null) return false;

        task.DeletedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<TaskColumnResponse>?> MoveTaskAsync(int id, MoveTaskRequest req)
    {
        var task = await db.TaskItems
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == DefaultUserId);

        if (task is null) return null;

        var targetColumnExists = await db.TaskColumns
            .AnyAsync(c => c.Id == req.ColumnId && c.UserId == DefaultUserId);
        if (!targetColumnExists) return null;

        var sourceColumnId = task.ColumnId;

        // Load tasks in both affected columns
        var affectedColumnIds = new HashSet<int> { sourceColumnId, req.ColumnId };
        var affectedTasks = await db.TaskItems
            .Where(t => affectedColumnIds.Contains(t.ColumnId) && t.UserId == DefaultUserId)
            .OrderBy(t => t.Position)
            .ToListAsync();

        // Remove the moving task from its source list
        var sourceTasks = affectedTasks
            .Where(t => t.ColumnId == sourceColumnId && t.Id != id)
            .OrderBy(t => t.Position)
            .ToList();

        // Only re-number source column when moving to a different column
        if (sourceColumnId != req.ColumnId)
        {
            for (int i = 0; i < sourceTasks.Count; i++)
                sourceTasks[i].Position = i;
        }

        // Build target list (excluding moving task if same column)
        var targetTasks = affectedTasks
            .Where(t => t.ColumnId == req.ColumnId && t.Id != id)
            .OrderBy(t => t.Position)
            .ToList();

        // Clamp position to valid range
        var insertAt = Math.Clamp(req.Position, 0, targetTasks.Count);

        // Update the moving task
        task.ColumnId = req.ColumnId;

        // Insert and reassign positions in target column
        targetTasks.Insert(insertAt, task);
        for (int i = 0; i < targetTasks.Count; i++)
            targetTasks[i].Position = i;

        await db.SaveChangesAsync();

        return await GetColumnsAsync();
    }

    private static TaskColumnResponse MapColumnResponse(TaskColumn col) =>
        new(
            Id: col.Id,
            Name: col.Name,
            Color: col.Color,
            Position: col.Position,
            Tasks: col.Tasks.Select(t => MapItemResponse(t, col.Name)).ToList()
        );

    private static TaskItemResponse MapItemResponse(TaskItem t, string colName) =>
        new(
            Id: t.Id,
            Title: t.Title,
            Description: t.Description,
            Priority: t.Priority,
            Project: t.Project,
            Due: t.DueDate?.ToString("yyyy-MM-dd"),
            ColName: colName,
            ColumnId: t.ColumnId,
            Position: t.Position
        );

    private static TaskDetailResponse MapDetailResponse(TaskItem t) =>
        new(
            Id: t.Id,
            Title: t.Title,
            Description: t.Description,
            Priority: t.Priority,
            Project: t.Project,
            Due: t.DueDate?.ToString("yyyy-MM-dd"),
            ColumnId: t.ColumnId,
            ColName: t.Column.Name,
            Position: t.Position
        );

    private static string NormalizePriority(string? priority) =>
        priority switch { "high" => "high", "low" => "low", _ => "med" };

    private static DateOnly? ParseDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        return DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
    }
}
