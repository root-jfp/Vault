using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class ChoreService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<ChoreWeekSummaryResponse> GetWeekAsync(string? week = null)
    {
        var (weekStart, weekEnd) = ParseWeek(week);
        var chores = await db.Chores
            .Where(c => c.UserId == DefaultUserId)
            .Include(c => c.Logs.Where(l => l.CompletedAt >= weekStart && l.CompletedAt < weekEnd))
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToListAsync();

        var responses = chores.Select(c =>
        {
            var log = c.Logs.FirstOrDefault();
            return new ChoreResponse(
                c.Id, c.Name, c.AssignedTo, c.Frequency, c.DayOfWeek,
                c.EffortPoints, c.Room, c.RotationType,
                log is not null, log?.CompletedBy);
        }).ToList();

        // Fairness score
        var joseLogs = chores.Where(c => c.Logs.Any()).Sum(c => c.EffortPoints);
        // Simplified: score by who completed
        int joseScore = 0, anaScore = 0;
        foreach (var c in chores)
        {
            var log = c.Logs.FirstOrDefault();
            if (log is null) continue;
            if (log.CompletedBy == "José") joseScore += c.EffortPoints;
            else anaScore += c.EffortPoints;
        }
        int total     = joseScore + anaScore;
        int fairness  = total > 0 ? (int)Math.Round(100.0 * Math.Min(joseScore, anaScore) / Math.Max(joseScore, anaScore)) : 100;

        return new ChoreWeekSummaryResponse(weekStart.ToString("yyyy-'W'WW"), joseScore, anaScore, fairness, responses);
    }

    public async Task<IReadOnlyList<ChoreLog>> GetChoreHistoryAsync(int id) =>
        await db.ChoreLogs.Where(l => l.ChoreId == id)
            .OrderByDescending(l => l.CompletedAt).Take(20)
            .AsNoTracking().ToListAsync();

    public async Task<ChoreResponse?> GetByIdAsync(int id)
    {
        var c = await db.Chores.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.UserId == DefaultUserId);
        if (c is null) return null;
        return new ChoreResponse(c.Id, c.Name, c.AssignedTo, c.Frequency, c.DayOfWeek, c.EffortPoints, c.Room, c.RotationType, false, null);
    }

    public async Task<ChoreResponse?> CreateAsync(CreateChoreRequest req)
    {
        var chore = new Chore
        {
            UserId       = DefaultUserId,
            Name         = req.Name,
            AssignedTo   = req.AssignedTo ?? "José",
            Frequency    = req.Frequency  ?? "weekly",
            DayOfWeek    = req.DayOfWeek,
            EffortPoints = req.EffortPoints ?? 1,
            Room         = req.Room,
            RotationType = req.RotationType ?? "none",
        };
        db.Chores.Add(chore);
        await db.SaveChangesAsync();
        return new ChoreResponse(chore.Id, chore.Name, chore.AssignedTo, chore.Frequency, chore.DayOfWeek, chore.EffortPoints, chore.Room, chore.RotationType, false, null);
    }

    public async Task<ChoreResponse?> UpdateAsync(int id, UpdateChoreRequest req)
    {
        var c = await db.Chores.FirstOrDefaultAsync(c => c.Id == id && c.UserId == DefaultUserId);
        if (c is null) return null;
        if (req.Name         is not null) c.Name         = req.Name;
        if (req.AssignedTo   is not null) c.AssignedTo   = req.AssignedTo;
        if (req.Frequency    is not null) c.Frequency    = req.Frequency;
        if (req.DayOfWeek    is not null) c.DayOfWeek    = req.DayOfWeek;
        if (req.EffortPoints is not null) c.EffortPoints = req.EffortPoints.Value;
        if (req.Room         is not null) c.Room         = req.Room;
        if (req.RotationType is not null) c.RotationType = req.RotationType;
        c.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return new ChoreResponse(c.Id, c.Name, c.AssignedTo, c.Frequency, c.DayOfWeek, c.EffortPoints, c.Room, c.RotationType, false, null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await db.Chores.FirstOrDefaultAsync(c => c.Id == id && c.UserId == DefaultUserId);
        if (c is null) return false;
        c.DeletedAt = DateTime.UtcNow;
        c.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ChoreResponse?> CompleteAsync(int id, CompleteChoreRequest req)
    {
        var c = await db.Chores.FirstOrDefaultAsync(c => c.Id == id && c.UserId == DefaultUserId);
        if (c is null) return null;
        db.ChoreLogs.Add(new ChoreLog
        {
            ChoreId     = id,
            UserId      = DefaultUserId,
            CompletedBy = req.CompletedBy ?? c.AssignedTo,
            CompletedAt = DateTime.UtcNow,
            Note        = req.Note,
        });
        await db.SaveChangesAsync();
        return new ChoreResponse(c.Id, c.Name, c.AssignedTo, c.Frequency, c.DayOfWeek, c.EffortPoints, c.Room, c.RotationType, true, req.CompletedBy ?? c.AssignedTo);
    }

    public async Task<ChoreResponse?> SwapAsync(int id, SwapChoreRequest req)
    {
        var c = await db.Chores.FirstOrDefaultAsync(c => c.Id == id && c.UserId == DefaultUserId);
        if (c is null) return null;
        c.AssignedTo = req.NewAssignee;
        c.UpdatedAt  = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return new ChoreResponse(c.Id, c.Name, c.AssignedTo, c.Frequency, c.DayOfWeek, c.EffortPoints, c.Room, c.RotationType, false, null);
    }

    private static (DateTime start, DateTime end) ParseWeek(string? week)
    {
        var today = DateTime.UtcNow.Date;
        var monday = today.AddDays(-(int)today.DayOfWeek + (today.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
        return (monday, monday.AddDays(7));
    }
}
