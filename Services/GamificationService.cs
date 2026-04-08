using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class GamificationService(VaultDbContext db)
{
    private static readonly int[] LevelThresholds = [0, 100, 250, 500, 800, 1200, 1700, 2300, 3000, 4000, int.MaxValue];
    private static readonly string[] LevelTitles   = ["Beginner","Starter","Consistent","Dedicated","Focused","Driven","Expert","Master","Champion","Legend"];

    public async Task<LeaderboardResponse> GetLeaderboardAsync(string period = "week")
    {
        var (from, to) = GetPeriodRange(period);
        var users = await db.UserProfiles.AsNoTracking().ToListAsync();

        var allXp = await db.XpEvents.AsNoTracking().ToListAsync();
        var periodXp = allXp.Where(x => x.CreatedAt >= from && x.CreatedAt <= to).ToList();

        var entries = new List<UserLeaderboardEntry>();
        foreach (var u in users)
        {
            var total  = allXp.Where(x => x.UserId == u.Id).Sum(x => x.Points);
            var pXp    = periodXp.Where(x => x.UserId == u.Id).Sum(x => x.Points);
            var (level, title, xpCurr, xpNext, xpInto) = ComputeLevel(total);
            var xpPct  = xpNext > xpCurr ? (int)Math.Round(100.0 * xpInto / (xpNext - xpCurr)) : 100;

            var habits = periodXp.Count(x => x.UserId == u.Id && x.Action == "habit_done");
            var tasks  = periodXp.Count(x => x.UserId == u.Id && x.Action == "task_done");
            var chores = periodXp.Count(x => x.UserId == u.Id && x.Action == "chore_done");

            entries.Add(new UserLeaderboardEntry(
                u.Id, u.Name, u.AvatarColor,
                total, level, title, xpCurr, xpNext, xpInto, xpPct,
                pXp, habits, tasks, chores));
        }

        return new LeaderboardResponse(period, entries.OrderByDescending(e => e.TotalXp).ToList());
    }

    public async Task<IReadOnlyList<XpEventResponse>> GetXpHistoryAsync(int days = 30)
    {
        var from = DateTime.UtcNow.AddDays(-days);
        var events = await db.XpEvents
            .Where(x => x.UserId == 1 && x.CreatedAt >= from)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking().ToListAsync();
        return events.Select(x => new XpEventResponse(
            x.Id, x.Action, x.Points, x.LinkedModule, x.LinkedId,
            x.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"))).ToList();
    }

    public async Task<IReadOnlyList<ChallengeResponse>> GetChallengesAsync()
    {
        var challenges = await db.Challenges
            .Include(c => c.Progress).ThenInclude(p => p.User)
            .AsNoTracking().ToListAsync();
        return challenges.Select(MapChallenge).ToList();
    }

    public async Task<ChallengeResponse> CreateChallengeAsync(CreateChallengeRequest req)
    {
        var challenge = new Challenge
        {
            CreatedByUserId = 1,
            Title           = req.Title,
            Description     = req.Description,
            TargetModule    = req.TargetModule,
            TargetCount     = req.TargetCount,
            StartDate       = ParseDate(req.StartDate) ?? DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate         = ParseDate(req.EndDate)   ?? DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
        };
        db.Challenges.Add(challenge);
        await db.SaveChangesAsync();
        return MapChallenge(challenge);
    }

    public async Task<bool> DeleteChallengeAsync(int id)
    {
        var c = await db.Challenges.FirstOrDefaultAsync(c => c.Id == id);
        if (c is null) return false;
        c.DeletedAt = DateTime.UtcNow;
        c.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task AwardXpAsync(int userId, string action, int points, string? module = null, int? linkedId = null)
    {
        db.XpEvents.Add(new XpEvent
        {
            UserId       = userId,
            Action       = action,
            Points       = points,
            LinkedModule = module,
            LinkedId     = linkedId,
        });
        await db.SaveChangesAsync();
    }

    private static (int level, string title, int xpCurr, int xpNext, int xpInto) ComputeLevel(int totalXp)
    {
        for (int i = 0; i < LevelThresholds.Length - 1; i++)
        {
            if (totalXp < LevelThresholds[i + 1])
            {
                var into = totalXp - LevelThresholds[i];
                var title = i < LevelTitles.Length ? LevelTitles[i] : "Legend";
                return (i + 1, title, LevelThresholds[i], LevelThresholds[i + 1], into);
            }
        }
        return (LevelTitles.Length, "Legend", LevelThresholds[^2], LevelThresholds[^1], 0);
    }

    private static ChallengeResponse MapChallenge(Challenge c) =>
        new(c.Id, c.Title, c.Description, c.TargetModule, c.TargetCount,
            c.StartDate.ToString("yyyy-MM-dd"), c.EndDate.ToString("yyyy-MM-dd"),
            c.Progress.Select(p => new ChallengeProgressResponse(
                p.UserId, p.User?.Name ?? "", p.CurrentCount, c.TargetCount,
                c.TargetCount > 0 ? (int)Math.Round(100.0 * p.CurrentCount / c.TargetCount) : 0)).ToList());

    private static (DateTime from, DateTime to) GetPeriodRange(string period)
    {
        var now = DateTime.UtcNow;
        return period switch
        {
            "month" => (new DateTime(now.Year, now.Month, 1), now),
            "year"  => (new DateTime(now.Year, 1, 1), now),
            _       => (now.AddDays(-6), now),
        };
    }

    private static DateOnly? ParseDate(string? s) =>
        DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
