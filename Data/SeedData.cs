using Microsoft.EntityFrameworkCore;
using Vault.Data.Models;

namespace Vault.Data;

public static class SeedData
{
    public static async Task InitializeAsync(VaultDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.UserProfiles.AnyAsync())
            return;

        var user = new UserProfile { Id = 1, Name = "José", AvatarColor = "#1D9E75" };
        db.UserProfiles.Add(user);

        var taskColumns = new List<TaskColumn>
        {
            new() { Id = 1, UserId = 1, Name = "Backlog",     Color = "var(--tx3)", Position = 0 },
            new() { Id = 2, UserId = 1, Name = "To Do",       Color = "var(--inf)", Position = 1 },
            new() { Id = 3, UserId = 1, Name = "In Progress", Color = "var(--wn)",  Position = 2 },
            new() { Id = 4, UserId = 1, Name = "Done",        Color = "var(--ac)",  Position = 3 },
        };
        db.TaskColumns.AddRange(taskColumns);

        var taskItems = new List<TaskItem>
        {
            new() { Id = 1,  UserId = 1, ColumnId = 1, Title = "Fix IFS posting control",
                    Priority = "high", Project = "Vault App",     DueDate = new DateOnly(2026, 4, 10), Position = 0 },
            new() { Id = 2,  UserId = 1, ColumnId = 1, Title = "Review invoice module",
                    Priority = "med",  Project = "Vault App",     DueDate = new DateOnly(2026, 4, 15), Position = 1 },
            new() { Id = 3,  UserId = 1, ColumnId = 1, Title = "FERRO Q2 planning",
                    Priority = "low",  Project = "FERRO",         DueDate = null,                      Position = 2 },
            new() { Id = 4,  UserId = 1, ColumnId = 2, Title = "Add kanban drag-drop",
                    Priority = "high", Project = "Vault App",     DueDate = new DateOnly(2026, 4, 8),  Position = 0 },
            new() { Id = 5,  UserId = 1, ColumnId = 2, Title = "Setup EF Core migrations",
                    Priority = "med",  Project = "Vault App",     DueDate = new DateOnly(2026, 4, 9),  Position = 1 },
            new() { Id = 6,  UserId = 1, ColumnId = 2, Title = "Order gym equipment",
                    Priority = "low",  Project = "FERRO",         DueDate = new DateOnly(2026, 4, 20), Position = 2 },
            new() { Id = 7,  UserId = 1, ColumnId = 3, Title = "Build habits page",
                    Priority = "high", Project = "Vault App",     DueDate = new DateOnly(2026, 4, 7),  Position = 0 },
            new() { Id = 8,  UserId = 1, ColumnId = 3, Title = "ASP.NET Core chapter 4",
                    Priority = "med",  Project = "Learning",      DueDate = new DateOnly(2026, 4, 10), Position = 1 },
            new() { Id = 9,  UserId = 1, ColumnId = 4, Title = "Vault CSS design system",
                    Priority = "high", Project = "Vault App",     DueDate = null,                      Position = 0 },
            new() { Id = 10, UserId = 1, ColumnId = 4, Title = "Dashboard layout",
                    Priority = "med",  Project = "Vault App",     DueDate = null,                      Position = 1 },
        };
        db.TaskItems.AddRange(taskItems);

        await db.SaveChangesAsync();
    }
}
