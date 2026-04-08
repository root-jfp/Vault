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

        var habits = new List<Habit>
        {
            new() { Id = 1, UserId = 1, Name = "Drink 2.5L Water", Type = "quant",   Frequency = "Daily", Color = "var(--inf)", HexColor = "#378ADD", WeeklyGoal = 7, Goal = 2500, UnitOfMeasure = "ml",    Increment = 250, SortOrder = 1, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 2, UserId = 1, Name = "Morning Prayers",  Type = "boolean", Frequency = "Daily", Color = "var(--wn)",  HexColor = "#EF9F27", WeeklyGoal = 7, Goal = 0,    UnitOfMeasure = null,    Increment = 1,   SortOrder = 2, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 3, UserId = 1, Name = "Workout",          Type = "boolean", Frequency = "Daily", Color = "var(--ac)",  HexColor = "#1D9E75", WeeklyGoal = 5, Goal = 0,    UnitOfMeasure = null,    Increment = 1,   SortOrder = 3, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 4, UserId = 1, Name = "Study 1h",         Type = "quant",   Frequency = "Daily", Color = "var(--pu)",  HexColor = "#7F77DD", WeeklyGoal = 7, Goal = 60,   UnitOfMeasure = "min",   Increment = 5,   SortOrder = 4, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 5, UserId = 1, Name = "Walk 6k Steps",    Type = "quant",   Frequency = "Daily", Color = "var(--ac)",  HexColor = "#5BC4C4", WeeklyGoal = 7, Goal = 6000, UnitOfMeasure = "steps", Increment = 500, SortOrder = 5, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 6, UserId = 1, Name = "Evening Prayers",  Type = "boolean", Frequency = "Daily", Color = "var(--pu)",  HexColor = "#C77DD3", WeeklyGoal = 7, Goal = 0,    UnitOfMeasure = null,    Increment = 1,   SortOrder = 6, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 7, UserId = 1, Name = "Go to Bed Early",  Type = "boolean", Frequency = "Daily", Color = "var(--wn)",  HexColor = "#E07B54", WeeklyGoal = 7, Goal = 0,    UnitOfMeasure = null,    Increment = 1,   SortOrder = 7, DaysOfWeek = "0,1,2,3,4,5,6" },
        };
        db.Habits.AddRange(habits);

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
            new() { Id = 7,  UserId = 1, ColumnId = 2, Title = "Finance admin",
                    Description = "- Access key for Maura\n- Declaration of no debt\n- Update family aggregate in José\n- Update civil status\n- Redo IRS 2024",
                    Priority = "high", Project = "Finances",  DueDate = null, Position = 4 },
            new() { Id = 8,  UserId = 1, ColumnId = 2, Title = "Go to Santander — create joint account",
                    Priority = "med",  Project = "Finances",  DueDate = null, Position = 5 },
        };
        db.TaskItems.AddRange(taskItems);

        await db.SaveChangesAsync();
    }
}
