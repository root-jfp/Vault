using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class PantryService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<PantryItemResponse>> GetAllAsync()
    {
        var items = await db.PantryItems
            .Where(p => p.UserId == DefaultUserId)
            .OrderBy(p => p.Category).ThenBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync();
        return items.Select(MapResponse).ToList();
    }

    public async Task<PantryStatsResponse> GetStatsAsync()
    {
        var items = await GetAllAsync();
        return new PantryStatsResponse(
            items.Count,
            items.Count(i => i.StockStatus == "low"),
            items.Count(i => i.StockStatus == "expiring"),
            items.Count(i => i.StockStatus == "out"));
    }

    public async Task<PantryItemResponse?> GetByIdAsync(int id)
    {
        var p = await db.PantryItems.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        return p is null ? null : MapResponse(p);
    }

    public async Task<PantryItemResponse> CreateAsync(CreatePantryItemRequest req)
    {
        var item = new PantryItem
        {
            UserId   = DefaultUserId,
            Name     = req.Name,
            Category = req.Category ?? "Other",
            Location = req.Location ?? "pantry",
            Quantity = req.Quantity,
            Unit     = req.Unit,
            MinStock = req.MinStock,
            ExpiryDate = ParseDate(req.ExpiryDate),
            Note     = req.Note,
        };
        db.PantryItems.Add(item);
        await db.SaveChangesAsync();
        return MapResponse(item);
    }

    public async Task<PantryItemResponse?> UpdateAsync(int id, UpdatePantryItemRequest req)
    {
        var item = await db.PantryItems.FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        if (item is null) return null;
        if (req.Name       is not null) item.Name       = req.Name;
        if (req.Category   is not null) item.Category   = req.Category;
        if (req.Location   is not null) item.Location   = req.Location;
        if (req.Quantity   is not null) item.Quantity   = req.Quantity.Value;
        if (req.Unit       is not null) item.Unit       = req.Unit;
        if (req.MinStock   is not null) item.MinStock   = req.MinStock;
        if (req.ExpiryDate is not null) item.ExpiryDate = ParseDate(req.ExpiryDate);
        if (req.Note       is not null) item.Note       = req.Note;
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapResponse(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await db.PantryItems.FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        if (item is null) return false;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<PantryItemResponse?> AdjustAsync(int id, AdjustPantryRequest req)
    {
        var item = await db.PantryItems.FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        if (item is null) return null;
        item.Quantity  = Math.Max(0, item.Quantity + req.Delta);
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapResponse(item);
    }

    private static PantryItemResponse MapResponse(PantryItem p)
    {
        var today  = DateOnly.FromDateTime(DateTime.UtcNow);
        string status;
        if (p.Quantity <= 0)
            status = "out";
        else if (p.MinStock.HasValue && p.Quantity <= p.MinStock.Value)
            status = "low";
        else if (p.ExpiryDate.HasValue && p.ExpiryDate.Value <= today.AddDays(3))
            status = "expiring";
        else
            status = "ok";

        return new PantryItemResponse(
            p.Id, p.Name, p.Category, p.Location,
            p.Quantity, p.Unit, p.MinStock,
            p.ExpiryDate?.ToString("yyyy-MM-dd"), p.Note, status);
    }

    private static DateOnly? ParseDate(string? s) =>
        DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
