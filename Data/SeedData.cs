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
            // Backlog
            new() { Id = 1,  UserId = 1, ColumnId = 1, Title = "Schedule movie night for Hail Mary",
                    Priority = "low",  Project = "Personal",  DueDate = null, Position = 0 },
            new() { Id = 2,  UserId = 1, ColumnId = 1, Title = "Schedule meetup with Carolina",
                    Priority = "low",  Project = "Personal",  DueDate = null, Position = 1 },
            // To Do — Housing
            new() { Id = 3,  UserId = 1, ColumnId = 2, Title = "Call pharmacy guy about house",
                    Priority = "med",  Project = "Housing",   DueDate = null, Position = 0 },
            new() { Id = 4,  UserId = 1, ColumnId = 2, Title = "Call landlord about leaks",
                    Priority = "high", Project = "Housing",   DueDate = null, Position = 1 },
            new() { Id = 5,  UserId = 1, ColumnId = 2, Title = "Create housing assistance request",
                    Priority = "med",  Project = "Housing",   DueDate = null, Position = 2 },
            new() { Id = 6,  UserId = 1, ColumnId = 2, Title = "Municipal assistance for childbirth",
                    Priority = "high", Project = "Housing",   DueDate = null, Position = 3 },
            // To Do — Finances
            new() { Id = 7,  UserId = 1, ColumnId = 2, Title = "Access key for Maura",
                    Priority = "med",  Project = "Finances",  DueDate = null, Position = 4 },
            new() { Id = 8,  UserId = 1, ColumnId = 2, Title = "Declaration of no debt",
                    Priority = "high", Project = "Finances",  DueDate = null, Position = 5 },
            new() { Id = 9,  UserId = 1, ColumnId = 2, Title = "Update family aggregate in José",
                    Priority = "med",  Project = "Finances",  DueDate = null, Position = 6 },
            new() { Id = 10, UserId = 1, ColumnId = 2, Title = "Update civil status",
                    Priority = "high", Project = "Finances",  DueDate = null, Position = 7 },
            new() { Id = 11, UserId = 1, ColumnId = 2, Title = "Redo IRS 2024",
                    Priority = "high", Project = "Finances",  DueDate = null, Position = 8 },
            new() { Id = 12, UserId = 1, ColumnId = 2, Title = "Go to Santander — create joint account",
                    Priority = "med",  Project = "Finances",  DueDate = null, Position = 9 },
        };
        db.TaskItems.AddRange(taskItems);

        await db.SaveChangesAsync();
    }
}
