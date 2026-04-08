using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class HabitService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<HabitResponse>> GetAllAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var thirtyDaysAgo = today.AddDays(-30);

        var habits = await db.Habits
            .Where(h => h.UserId == DefaultUserId)
            .OrderBy(h => h.SortOrder)
            .Include(h => h.Entries.Where(e => e.Date >= thirtyDaysAgo))
            .AsNoTracking()
            .ToListAsync();

        // Load all entry dates for accurate streak calculation (not capped at 30 days)
        var allDates = await db.HabitEntries
            .Where(e => e.UserId == DefaultUserId)
            .Select(e => new { e.HabitId, e.Date })
            .AsNoTracking()
            .ToListAsync();
        var streakDatesByHabit = allDates
            .GroupBy(e => e.HabitId)
            .ToDictionary(g => g.Key, g => (ICollection<DateOnly>)g.Select(e => e.Date).ToHashSet());

        return habits.Select(h =>
        {
            var todayEntry = h.Entries.FirstOrDefault(e => e.Date == today);
            var weekEntries = h.Entries.Where(e => e.Date >= weekStart && e.Date <= today).ToList();
            var streakDates = streakDatesByHabit.GetValueOrDefault(h.Id, []);
            var streak = CalculateStreak(streakDates, today);
            var rate = CalculateRate(h.Entries, today, thirtyDaysAgo);
            var displayTarget = MapDisplayTarget(h.Goal, h.UnitOfMeasure);
            var displayUnit = MapDisplayUnit(h.UnitOfMeasure, h.Goal);

            return new HabitResponse(
                Id: h.Id,
                Name: h.Name,
                Color: h.Color,
                Hex: h.HexColor,
                Type: h.Type == "boolean" ? "bool" : "quant",
                Freq: h.Frequency,
                Streak: streak,
                Pct: rate,
                Done: todayEntry is not null,
                Qty: todayEntry is not null ? MapDisplayTarget(todayEntry.Amount, h.UnitOfMeasure) : 0,
                Target: displayTarget,
                Unit: displayUnit,
                WeekDone: weekEntries.Count,
                WeekGoal: h.WeeklyGoal,
                Icon: h.Icon,
                Notes: h.Notes,
                HmView: "week"
            );
        }).ToList();
    }

    public async Task<HabitDetailResponse?> GetByIdAsync(int id)
    {
        var habit = await db.Habits
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == DefaultUserId);

        if (habit is null) return null;

        return new HabitDetailResponse(
            Id: habit.Id,
            Name: habit.Name,
            Type: habit.Type,
            Freq: habit.Frequency,
            Color: habit.HexColor,
            Icon: habit.Icon,
            Goal: habit.Goal,
            Uom: habit.UnitOfMeasure,
            Increment: habit.Increment,
            StartDate: habit.StartDate?.ToString("yyyy-MM-dd"),
            WeeklyGoal: habit.WeeklyGoal,
            Notes: habit.Notes
        );
    }

    public async Task<HabitDetailResponse> CreateAsync(CreateHabitRequest req)
    {
        var maxSort = await db.Habits
            .Where(h => h.UserId == DefaultUserId)
            .MaxAsync(h => (int?)h.SortOrder) ?? 0;

        var colorMap = BuildColorMap();
        var cssVar = colorMap.GetValueOrDefault(req.Color, "var(--ac)");

        var habit = new Habit
        {
            UserId = DefaultUserId,
            Name = req.Name,
            Type = req.Type == "quant" ? "quant" : "boolean",
            Frequency = req.Freq,
            Color = cssVar,
            HexColor = req.Color,
            WeeklyGoal = req.WeeklyGoal ?? 7,
            Goal = req.Goal ?? 0,
            UnitOfMeasure = req.Uom,
            Increment = req.Increment ?? 1,
            Icon = req.Icon,
            Notes = req.Notes,
            StartDate = req.StartDate is not null && DateTime.TryParseExact(req.StartDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var sd) ? sd : null,
            SortOrder = maxSort + 1,
        };

        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        return (await GetByIdAsync(habit.Id))!;
    }

    public async Task<HabitDetailResponse?> UpdateAsync(int id, UpdateHabitRequest req)
    {
        var habit = await db.Habits
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == DefaultUserId);

        if (habit is null) return null;

        if (req.Name is not null) habit.Name = req.Name;
        if (req.Type is not null) habit.Type = req.Type == "quant" ? "quant" : "boolean";
        if (req.Freq is not null) habit.Frequency = req.Freq;
        if (req.Color is not null)
        {
            var colorMap = BuildColorMap();
            habit.HexColor = req.Color;
            habit.Color = colorMap.GetValueOrDefault(req.Color, "var(--ac)");
        }
        if (req.Icon is not null) habit.Icon = req.Icon;
        if (req.Goal is not null) habit.Goal = req.Goal.Value;
        if (req.Uom is not null) habit.UnitOfMeasure = req.Uom;
        if (req.Increment is not null) habit.Increment = req.Increment.Value;
        if (req.WeeklyGoal is not null) habit.WeeklyGoal = req.WeeklyGoal.Value;
        if (req.Notes is not null) habit.Notes = req.Notes;
        if (req.StartDate is not null &&
            DateTime.TryParseExact(req.StartDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var sd))
            habit.StartDate = sd;
        habit.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var habit = await db.Habits
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == DefaultUserId);

        if (habit is null) return false;

        habit.DeletedAt = DateTime.UtcNow;
        habit.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<HabitResponse?> ToggleAsync(int id, ToggleHabitRequest req)
    {
        var habit = await db.Habits
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == DefaultUserId);

        if (habit is null) return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existingEntry = await db.HabitEntries
            .FirstOrDefaultAsync(e => e.HabitId == id && e.Date == today);

        if (habit.Type == "boolean")
        {
            if (existingEntry is not null)
                db.HabitEntries.Remove(existingEntry);
            else
                db.HabitEntries.Add(new HabitEntry
                {
                    HabitId = id, UserId = DefaultUserId, Date = today, Count = 1, Amount = 1,
                });
        }
        else
        {
            // For quant: amount is in display units (L), convert back to raw (ml) if needed
            var rawAmount = ConvertToRaw(req.Amount ?? 0, habit.UnitOfMeasure, habit.Goal);
            if (rawAmount <= 0 && existingEntry is not null)
            {
                db.HabitEntries.Remove(existingEntry);
            }
            else if (rawAmount > 0)
            {
                if (existingEntry is not null)
                {
                    existingEntry.Amount = rawAmount;
                    existingEntry.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    db.HabitEntries.Add(new HabitEntry
                    {
                        HabitId = id, UserId = DefaultUserId, Date = today, Count = 1, Amount = rawAmount,
                    });
                }
            }
        }

        await db.SaveChangesAsync();

        var all = await GetAllAsync();
        return all.FirstOrDefault(h => h.Id == id);
    }

    public async Task<IReadOnlyList<HeatmapCell>> GetHeatmapAsync(int id, int year, int month)
    {
        var habit = await db.Habits
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == DefaultUserId);

        if (habit is null) return [];

        var startDate = new DateOnly(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var entries = await db.HabitEntries
            .Where(e => e.HabitId == id && e.Date >= startDate && e.Date <= endDate)
            .AsNoTracking()
            .ToDictionaryAsync(e => e.Date);

        var cells = new List<HeatmapCell>();

        var firstDow = (int)startDate.DayOfWeek;
        for (int i = 0; i < firstDow; i++)
            cells.Add(new HeatmapCell(0, true, false, false, 0));

        for (int day = 1; day <= endDate.Day; day++)
        {
            var date = new DateOnly(year, month, day);
            var isToday = date == today;
            var future = date > today;
            var level = 0;

            if (!future && entries.TryGetValue(date, out var entry))
            {
                level = habit.Type == "boolean" ? 4 : (habit.Goal > 0 ? entry.Amount / habit.Goal : 0) switch
                {
                    >= 1.0 => 4,
                    >= 0.75 => 3,
                    >= 0.50 => 2,
                    _ => 1,
                };
            }

            cells.Add(new HeatmapCell(day, false, isToday, future, level));
        }

        return cells;
    }

    private static int CalculateStreak(ICollection<DateOnly> dates, DateOnly today)
    {
        var streak = 0;
        var dateSet = dates is HashSet<DateOnly> hs ? hs : dates.ToHashSet();

        var checkDate = dateSet.Contains(today) ? today : today.AddDays(-1);

        while (dateSet.Contains(checkDate))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }

        return streak;
    }

    private static int CalculateRate(ICollection<HabitEntry> entries, DateOnly today, DateOnly periodStart)
    {
        var totalDays = today.DayNumber - periodStart.DayNumber + 1;
        if (totalDays <= 0) return 0;
        var doneCount = entries.Count(e => e.Date >= periodStart && e.Date <= today);
        return (int)Math.Round(100.0 * doneCount / totalDays);
    }

    private static double MapDisplayTarget(double goal, string? uom)
    {
        if (uom == "ml" && goal >= 1000) return goal / 1000;
        return goal;
    }

    private static string? MapDisplayUnit(string? uom, double goal)
    {
        if (uom == "ml" && goal >= 1000) return "L";
        return uom;
    }

    private static double ConvertToRaw(double displayValue, string? uom, double goal)
    {
        if (uom == "ml" && goal >= 1000) return displayValue * 1000;
        return displayValue;
    }

    private static Dictionary<string, string> BuildColorMap() => new()
    {
        ["#1D9E75"] = "var(--ac)",
        ["#378ADD"] = "var(--inf)",
        ["#EF9F27"] = "var(--wn)",
        ["#7F77DD"] = "var(--pu)",
        ["#E24B4A"] = "var(--dn)",
    };
}
