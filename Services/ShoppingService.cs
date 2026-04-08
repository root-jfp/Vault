using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class ShoppingService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<ShoppingListSummaryResponse>> GetAllListsAsync()
    {
        var lists = await db.ShoppingLists
            .Where(l => l.UserId == DefaultUserId)
            .Include(l => l.Items)
            .OrderByDescending(l => l.IsDefault)
            .AsNoTracking()
            .ToListAsync();
        return lists.Select(l => new ShoppingListSummaryResponse(
            l.Id, l.Name, l.Color, l.IsDefault, l.Items.Count, l.Items.Count(i => i.IsDone))).ToList();
    }

    public async Task<ShoppingListDetailResponse?> GetListAsync(int listId)
    {
        var list = await db.ShoppingLists
            .Include(l => l.Items.OrderBy(i => i.SortOrder))
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == listId && l.UserId == DefaultUserId);
        if (list is null) return null;
        var items = list.Items.Select(MapItem).ToList();
        return new ShoppingListDetailResponse(
            list.Id, list.Name, list.Color, list.IsDefault,
            list.Items.Count, list.Items.Count(i => i.IsDone), items);
    }

    public async Task<ShoppingListDetailResponse?> GetDefaultListAsync()
    {
        var defaultList = await db.ShoppingLists
            .Where(l => l.UserId == DefaultUserId && l.IsDefault)
            .AsNoTracking()
            .FirstOrDefaultAsync()
            ?? await db.ShoppingLists.Where(l => l.UserId == DefaultUserId).AsNoTracking().FirstOrDefaultAsync();
        if (defaultList is null) return null;
        return await GetListAsync(defaultList.Id);
    }

    public async Task<ShoppingListSummaryResponse> CreateListAsync(CreateShoppingListRequest req)
    {
        var list = new ShoppingList
        {
            UserId    = DefaultUserId,
            Name      = req.Name,
            Color     = req.Color,
            IsDefault = req.IsDefault ?? false,
        };
        db.ShoppingLists.Add(list);
        await db.SaveChangesAsync();
        return new ShoppingListSummaryResponse(list.Id, list.Name, list.Color, list.IsDefault, 0, 0);
    }

    public async Task<bool> DeleteListAsync(int id)
    {
        var list = await db.ShoppingLists.FirstOrDefaultAsync(l => l.Id == id && l.UserId == DefaultUserId);
        if (list is null) return false;
        list.DeletedAt = DateTime.UtcNow;
        list.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ShoppingItemResponse?> AddItemAsync(int listId, CreateShoppingItemRequest req)
    {
        var listExists = await db.ShoppingLists.AnyAsync(l => l.Id == listId && l.UserId == DefaultUserId);
        if (!listExists) return null;
        var maxSort = await db.ShoppingItems.Where(i => i.ListId == listId).MaxAsync(i => (int?)i.SortOrder) ?? -1;
        var item = new ShoppingItem
        {
            ListId      = listId,
            UserId      = DefaultUserId,
            Name        = req.Name,
            Quantity    = req.Quantity,
            Unit        = req.Unit,
            Category    = req.Category,
            Note        = req.Note,
            IsRecurring = req.IsRecurring ?? false,
            SortOrder   = maxSort + 1,
        };
        db.ShoppingItems.Add(item);
        await db.SaveChangesAsync();
        return MapItem(item);
    }

    public async Task<ShoppingItemResponse?> UpdateItemAsync(int id, UpdateShoppingItemRequest req)
    {
        var item = await db.ShoppingItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == DefaultUserId);
        if (item is null) return null;
        if (req.Name        is not null) item.Name        = req.Name;
        if (req.Quantity    is not null) item.Quantity    = req.Quantity;
        if (req.Unit        is not null) item.Unit        = req.Unit;
        if (req.Category    is not null) item.Category    = req.Category;
        if (req.Note        is not null) item.Note        = req.Note;
        if (req.IsRecurring is not null) item.IsRecurring = req.IsRecurring.Value;
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapItem(item);
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await db.ShoppingItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == DefaultUserId);
        if (item is null) return false;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ShoppingItemResponse?> ToggleItemAsync(int id)
    {
        var item = await db.ShoppingItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == DefaultUserId);
        if (item is null) return null;
        item.IsDone    = !item.IsDone;
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapItem(item);
    }

    public async Task<int> ClearDoneItemsAsync(int listId)
    {
        var items = await db.ShoppingItems
            .Where(i => i.ListId == listId && i.UserId == DefaultUserId && i.IsDone)
            .ToListAsync();
        foreach (var item in items)
        {
            item.DeletedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();
        return items.Count;
    }

    private static ShoppingItemResponse MapItem(ShoppingItem i) =>
        new(i.Id, i.ListId, i.Name, i.Quantity, i.Unit, i.Category, i.Note, i.IsDone, i.IsRecurring, i.SortOrder);
}
