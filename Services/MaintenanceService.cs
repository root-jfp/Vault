using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class MaintenanceService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<MaintenanceItemResponse>> GetAllAsync()
    {
        var items = await db.MaintenanceItems
            .Where(m => m.UserId == DefaultUserId)
            .OrderBy(m => m.SortOrder)
            .AsNoTracking()
            .ToListAsync();
        return items.Select(MapResponse).ToList();
    }

    public async Task<MaintenanceItemDetailResponse?> GetByIdAsync(int id)
    {
        var item = await db.MaintenanceItems
            .Include(m => m.Logs.OrderByDescending(l => l.CompletedAt).Take(20))
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == DefaultUserId);
        if (item is null) return null;
        var logs = item.Logs.Select(l => new MaintenanceLogResponse(
            l.Id, l.CompletedAt.ToString("yyyy-MM-dd"), l.Note, l.Cost)).ToList();
        var resp = MapResponse(item);
        return new MaintenanceItemDetailResponse(
            resp.Id, resp.Name, resp.Category, resp.Room, resp.Icon,
            resp.IntervalDays, resp.LastCompletedAt, resp.NextDueAt, resp.Notes,
            resp.Status, resp.DaysUntilDue, resp.OverduePercent, logs);
    }

    public async Task<MaintenanceItemResponse> CreateAsync(CreateMaintenanceItemRequest req)
    {
        var maxSort = await db.MaintenanceItems.Where(m => m.UserId == DefaultUserId).MaxAsync(m => (int?)m.SortOrder) ?? 0;
        var item = new MaintenanceItem
        {
            UserId      = DefaultUserId,
            Name        = req.Name,
            Category    = req.Category,
            Room        = req.Room,
            Icon        = req.Icon,
            IntervalDays = req.IntervalDays,
            Notes       = req.Notes,
            SortOrder   = maxSort + 1,
        };
        db.MaintenanceItems.Add(item);
        await db.SaveChangesAsync();
        return MapResponse(item);
    }

    public async Task<MaintenanceItemResponse?> UpdateAsync(int id, UpdateMaintenanceItemRequest req)
    {
        var item = await db.MaintenanceItems.FirstOrDefaultAsync(m => m.Id == id && m.UserId == DefaultUserId);
        if (item is null) return null;
        if (req.Name         is not null) item.Name         = req.Name;
        if (req.Category     is not null) item.Category     = req.Category;
        if (req.Room         is not null) item.Room         = req.Room;
        if (req.Icon         is not null) item.Icon         = req.Icon;
        if (req.IntervalDays is not null) item.IntervalDays = req.IntervalDays.Value;
        if (req.Notes        is not null) item.Notes        = req.Notes;
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapResponse(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await db.MaintenanceItems.FirstOrDefaultAsync(m => m.Id == id && m.UserId == DefaultUserId);
        if (item is null) return false;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<MaintenanceItemResponse?> CompleteAsync(int id, CompleteMaintenanceRequest req)
    {
        var item = await db.MaintenanceItems.FirstOrDefaultAsync(m => m.Id == id && m.UserId == DefaultUserId);
        if (item is null) return null;
        var now     = DateTime.UtcNow;
        var nextDue = now.AddDays(item.IntervalDays);
        item.LastCompletedAt = now;
        item.NextDueAt       = nextDue;
        item.UpdatedAt       = now;

        db.MaintenanceLogs.Add(new MaintenanceLog
        {
            ItemId      = id,
            UserId      = DefaultUserId,
            CompletedAt = now,
            Note        = req.Note,
            Cost        = req.Cost,
        });
        await db.SaveChangesAsync();
        return MapResponse(item);
    }

    private static MaintenanceItemResponse MapResponse(MaintenanceItem item)
    {
        var today      = DateTime.UtcNow.Date;
        var nextDue    = item.NextDueAt?.Date;
        int daysUntil  = nextDue.HasValue ? (int)(nextDue.Value - today).TotalDays : 0;
        string status  = daysUntil < 0 ? "overdue" : daysUntil <= 3 ? "soon" : "ok";
        int overduePct = 0;
        if (item.NextDueAt.HasValue && item.LastCompletedAt.HasValue && daysUntil < 0)
        {
            var totalSpan = (item.NextDueAt.Value - item.LastCompletedAt.Value).TotalDays;
            var overdue   = -daysUntil;
            overduePct    = totalSpan > 0 ? (int)Math.Min(100, Math.Round(100.0 * overdue / totalSpan)) : 100;
        }
        return new MaintenanceItemResponse(
            item.Id, item.Name, item.Category, item.Room, item.Icon,
            item.IntervalDays,
            item.LastCompletedAt?.ToString("yyyy-MM-dd"),
            item.NextDueAt?.ToString("yyyy-MM-dd"),
            item.Notes, status, daysUntil, overduePct);
    }
}
