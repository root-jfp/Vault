using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;

namespace Vault.Services;

public sealed class PerformanceService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<PerformanceSummaryResponse> GetSummaryAsync(string period = "week")
    {
        var (from, to) = GetPeriodRange(period);

        // Habits
        var habits = await db.Habits.Where(h => h.UserId == DefaultUserId).AsNoTracking().ToListAsync();
        var entries = await db.HabitEntries
            .Where(e => e.UserId == DefaultUserId && e.Date >= from && e.Date <= to)
            .AsNoTracking().ToListAsync();
        var totalDays    = to.DayNumber - from.DayNumber + 1;
        var habitPerf    = habits.Select(h =>
        {
            var logged = entries.Count(e => e.HabitId == h.Id);
            var pct    = totalDays > 0 ? (int)Math.Round(100.0 * logged / totalDays) : 0;
            return new HabitPerformanceResponse(h.Id, h.Name, h.HexColor, pct, 0, logged, totalDays);
        }).ToList();
        int avgHabitPct = habitPerf.Count > 0 ? (int)habitPerf.Average(h => h.CompletionPct) : 0;

        // Tasks
        var doneColumn = await db.TaskColumns
            .Where(c => c.UserId == DefaultUserId && c.Name == "Done")
            .AsNoTracking().FirstOrDefaultAsync();
        var tasks = await db.TaskItems.Where(t => t.UserId == DefaultUserId).AsNoTracking().ToListAsync();
        int totalDone   = doneColumn is not null ? tasks.Count(t => t.ColumnId == doneColumn.Id) : 0;
        int openTasks   = tasks.Count(t => doneColumn is null || t.ColumnId != doneColumn.Id);
        int overdue     = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateOnly.FromDateTime(DateTime.UtcNow) && (doneColumn is null || t.ColumnId != doneColumn.Id));
        double velocity = totalDays > 0 ? Math.Round(7.0 * totalDone / totalDays, 1) : 0;
        var taskPerf    = new TaskPerformanceResponse(totalDone, totalDone, openTasks, overdue, velocity);

        // Maintenance
        var maintenance = await db.MaintenanceItems.Where(m => m.UserId == DefaultUserId).AsNoTracking().ToListAsync();
        var today       = DateOnly.FromDateTime(DateTime.UtcNow);
        int overdueM    = maintenance.Count(m => m.NextDueAt.HasValue && DateOnly.FromDateTime(m.NextDueAt.Value) < today);
        int onTime      = maintenance.Count - overdueM;
        int onTimePct   = maintenance.Count > 0 ? (int)Math.Round(100.0 * onTime / maintenance.Count) : 100;
        var maintPerf   = new MaintenancePerformanceResponse(maintenance.Count, onTime, overdueM, onTimePct);

        // Productivity score (weighted)
        int score = (int)Math.Round(0.4 * avgHabitPct + 0.4 * Math.Min(100, velocity * 10) + 0.2 * onTimePct);

        return new PerformanceSummaryResponse(
            period, score, avgHabitPct, (int)velocity, onTimePct, 0,
            habitPerf, taskPerf, maintPerf);
    }

    private static (DateOnly from, DateOnly to) GetPeriodRange(string period)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return period switch
        {
            "month" => (new DateOnly(today.Year, today.Month, 1), today),
            "year"  => (new DateOnly(today.Year, 1, 1), today),
            _       => (today.AddDays(-6), today),  // week default
        };
    }
}
