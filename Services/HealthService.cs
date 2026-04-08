using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class HealthService(VaultDbContext db)
{
    private const int DefaultUserId = 1;
    private const double WaterGoalMl = 2500;

    public async Task<IReadOnlyList<HealthMetricResponse>> GetMetricsAsync(string metricType, int days = 7)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from  = today.AddDays(-days + 1);
        var metrics = await db.HealthMetrics
            .Where(h => h.UserId == DefaultUserId && h.MetricType == metricType && h.Date >= from)
            .OrderBy(h => h.Date)
            .AsNoTracking()
            .ToListAsync();
        return metrics.Select(Map).ToList();
    }

    public async Task<HealthSummaryResponse> GetSummaryAsync(string metricType)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from7  = today.AddDays(-6);
        var from30 = today.AddDays(-29);

        var metrics = await db.HealthMetrics
            .Where(h => h.UserId == DefaultUserId && h.MetricType == metricType && h.Date >= from30)
            .OrderBy(h => h.Date)
            .AsNoTracking()
            .ToListAsync();

        var todayEntry = metrics.FirstOrDefault(h => h.Date == today);
        var todayValue = todayEntry?.Value ?? 0;
        var unit       = metrics.FirstOrDefault()?.Unit ?? DefaultUnit(metricType);
        var goal       = metricType == "water" ? WaterGoalMl : 0;
        var pct        = goal > 0 ? (int)Math.Min(100, Math.Round(100.0 * todayValue / goal)) : 0;

        var last7   = metrics.Where(h => h.Date >= from7).ToList();
        var avg7    = last7.Count > 0 ? last7.Average(h => h.Value) : 0;
        var avg30   = metrics.Count > 0 ? metrics.Average(h => h.Value) : 0;

        // Streak: consecutive days with an entry
        int streak = 0;
        for (var d = today; ; d = d.AddDays(-1))
        {
            if (!metrics.Any(h => h.Date == d)) break;
            streak++;
        }

        return new HealthSummaryResponse(metricType, todayValue, goal, pct, streak,
            Math.Round(avg7, 1), Math.Round(avg30, 1), unit);
    }

    public async Task<HealthMetricResponse> LogAsync(LogHealthRequest req)
    {
        var date = ParseDate(req.Date) ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var unit = req.Unit ?? DefaultUnit(req.MetricType);

        var existing = await db.HealthMetrics
            .FirstOrDefaultAsync(h => h.UserId == DefaultUserId && h.MetricType == req.MetricType && h.Date == date);

        if (existing is not null)
        {
            existing.Value     = req.Value;
            existing.Unit      = unit;
            existing.Note      = req.Note;
            existing.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Map(existing);
        }

        var metric = new HealthMetric
        {
            UserId     = DefaultUserId,
            MetricType = req.MetricType,
            Date       = date,
            Value      = req.Value,
            Unit       = unit,
            Note       = req.Note,
        };
        db.HealthMetrics.Add(metric);
        await db.SaveChangesAsync();
        return Map(metric);
    }

    public async Task<HealthMetricResponse?> UpdateAsync(int id, UpdateHealthRequest req)
    {
        var m = await db.HealthMetrics.FirstOrDefaultAsync(h => h.Id == id && h.UserId == DefaultUserId);
        if (m is null) return null;
        if (req.Value is not null) m.Value = req.Value.Value;
        if (req.Unit  is not null) m.Unit  = req.Unit;
        if (req.Note  is not null) m.Note  = req.Note;
        m.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Map(m);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var m = await db.HealthMetrics.FirstOrDefaultAsync(h => h.Id == id && h.UserId == DefaultUserId);
        if (m is null) return false;
        db.HealthMetrics.Remove(m);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<HealthMetricResponse> QuickAddWaterAsync(double amountMl)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existing = await db.HealthMetrics
            .FirstOrDefaultAsync(h => h.UserId == DefaultUserId && h.MetricType == "water" && h.Date == today);

        if (existing is not null)
        {
            existing.Value    += amountMl;
            existing.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Map(existing);
        }

        var metric = new HealthMetric
        {
            UserId     = DefaultUserId,
            MetricType = "water",
            Date       = today,
            Value      = amountMl,
            Unit       = "ml",
        };
        db.HealthMetrics.Add(metric);
        await db.SaveChangesAsync();
        return Map(metric);
    }

    private static HealthMetricResponse Map(HealthMetric h) =>
        new(h.Id, h.MetricType, h.Date.ToString("yyyy-MM-dd"), h.Value, h.Unit, h.Note);

    private static string DefaultUnit(string type) =>
        type switch { "water" => "ml", "weight" => "kg", "sleep" => "h", "hr" => "bpm", _ => "" };

    private static DateOnly? ParseDate(string? s) =>
        DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
