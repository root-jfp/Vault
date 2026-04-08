using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class BudgetService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<BudgetCategoryResponse>> GetCategoriesAsync(string? month = null)
    {
        var (year, mon) = ParseMonth(month);
        var categories = await db.BudgetCategories
            .Where(c => c.UserId == DefaultUserId)
            .OrderBy(c => c.SortOrder)
            .AsNoTracking()
            .ToListAsync();

        var spent = await db.Transactions
            .Where(t => t.UserId == DefaultUserId && t.Date.Year == year && t.Date.Month == mon)
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(t => t.Amount) })
            .AsNoTracking()
            .ToListAsync();

        var spentMap = spent.ToDictionary(x => x.CategoryId, x => x.Total);

        return categories.Select(c =>
        {
            var s   = spentMap.GetValueOrDefault(c.Id);
            var rem = c.MonthlyLimit - s;
            var pct = c.MonthlyLimit > 0 ? (int)Math.Min(100, Math.Round(100.0 * (double)s / (double)c.MonthlyLimit)) : 0;
            return new BudgetCategoryResponse(c.Id, c.Name, c.MonthlyLimit, c.Color, c.Icon, s, rem, pct, s > c.MonthlyLimit);
        }).ToList();
    }

    public async Task<BudgetSummaryResponse> GetSummaryAsync(string? month = null)
    {
        var (year, mon) = ParseMonth(month);
        var categories  = await GetCategoriesAsync(month);
        var totalBudget = categories.Sum(c => c.MonthlyLimit);
        var totalSpent  = categories.Sum(c => c.Spent);
        var remaining   = totalBudget - totalSpent;
        var savingsRate = totalBudget > 0 ? (int)Math.Round(100.0 * (double)remaining / (double)totalBudget) : 0;
        return new BudgetSummaryResponse(
            $"{year}-{mon:D2}", totalBudget, totalSpent, remaining, savingsRate, categories);
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetTransactionsAsync(string? month = null, int? categoryId = null)
    {
        var (year, mon) = ParseMonth(month);
        var query = db.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == DefaultUserId && t.Date.Year == year && t.Date.Month == mon);
        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);
        var txns = await query.OrderByDescending(t => t.Date).AsNoTracking().ToListAsync();
        return txns.Select(t => new TransactionResponse(
            t.Id, t.CategoryId, t.Category.Name, t.Amount,
            t.Description, t.Payee, t.Date.ToString("yyyy-MM-dd"), t.IsRecurring)).ToList();
    }

    public async Task<BudgetCategoryResponse?> CreateCategoryAsync(CreateCategoryRequest req)
    {
        var maxSort = await db.BudgetCategories.Where(c => c.UserId == DefaultUserId).MaxAsync(c => (int?)c.SortOrder) ?? 0;
        var cat = new BudgetCategory
        {
            UserId       = DefaultUserId,
            Name         = req.Name,
            MonthlyLimit = req.MonthlyLimit,
            Color        = req.Color,
            Icon         = req.Icon,
            SortOrder    = maxSort + 1,
        };
        db.BudgetCategories.Add(cat);
        await db.SaveChangesAsync();
        return new BudgetCategoryResponse(cat.Id, cat.Name, cat.MonthlyLimit, cat.Color, cat.Icon, 0, cat.MonthlyLimit, 0, false);
    }

    public async Task<BudgetCategoryResponse?> UpdateCategoryAsync(int id, UpdateCategoryRequest req)
    {
        var cat = await db.BudgetCategories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == DefaultUserId);
        if (cat is null) return null;
        if (req.Name         is not null) cat.Name         = req.Name;
        if (req.MonthlyLimit is not null) cat.MonthlyLimit = req.MonthlyLimit.Value;
        if (req.Color        is not null) cat.Color        = req.Color;
        if (req.Icon         is not null) cat.Icon         = req.Icon;
        cat.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return new BudgetCategoryResponse(cat.Id, cat.Name, cat.MonthlyLimit, cat.Color, cat.Icon, 0, cat.MonthlyLimit, 0, false);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var cat = await db.BudgetCategories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == DefaultUserId);
        if (cat is null) return false;
        cat.DeletedAt = DateTime.UtcNow;
        cat.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<TransactionResponse?> CreateTransactionAsync(CreateTransactionRequest req)
    {
        var catExists = await db.BudgetCategories.AnyAsync(c => c.Id == req.CategoryId && c.UserId == DefaultUserId);
        if (!catExists) return null;
        var date = ParseDate(req.Date) ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var txn = new Transaction
        {
            UserId        = DefaultUserId,
            CategoryId    = req.CategoryId,
            Amount        = req.Amount,
            Description   = req.Description,
            Payee         = req.Payee,
            Date          = date,
            IsRecurring   = req.IsRecurring ?? false,
        };
        db.Transactions.Add(txn);
        await db.SaveChangesAsync();
        var cat = await db.BudgetCategories.FindAsync(req.CategoryId);
        return new TransactionResponse(txn.Id, txn.CategoryId, cat!.Name, txn.Amount, txn.Description, txn.Payee, txn.Date.ToString("yyyy-MM-dd"), txn.IsRecurring);
    }

    public async Task<TransactionResponse?> UpdateTransactionAsync(int id, UpdateTransactionRequest req)
    {
        var txn = await db.Transactions.Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == DefaultUserId);
        if (txn is null) return null;
        if (req.CategoryId  is not null) txn.CategoryId  = req.CategoryId.Value;
        if (req.Amount      is not null) txn.Amount      = req.Amount.Value;
        if (req.Description is not null) txn.Description = req.Description;
        if (req.Payee       is not null) txn.Payee       = req.Payee;
        if (req.Date        is not null) txn.Date        = ParseDate(req.Date) ?? txn.Date;
        if (req.IsRecurring is not null) txn.IsRecurring = req.IsRecurring.Value;
        txn.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return new TransactionResponse(txn.Id, txn.CategoryId, txn.Category.Name, txn.Amount, txn.Description, txn.Payee, txn.Date.ToString("yyyy-MM-dd"), txn.IsRecurring);
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        var txn = await db.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == DefaultUserId);
        if (txn is null) return false;
        txn.DeletedAt = DateTime.UtcNow;
        txn.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    private static (int year, int month) ParseMonth(string? s)
    {
        if (s is not null && s.Length == 7 &&
            int.TryParse(s[..4], out var y) && int.TryParse(s[5..], out var m) && m >= 1 && m <= 12)
            return (y, m);
        var now = DateTime.UtcNow;
        return (now.Year, now.Month);
    }

    private static DateOnly? ParseDate(string? s) =>
        DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
