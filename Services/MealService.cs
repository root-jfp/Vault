using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class MealService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<RecipeResponse>> GetRecipesAsync()
    {
        var recipes = await db.Recipes
            .Where(r => r.UserId == DefaultUserId)
            .OrderByDescending(r => r.IsFavorite).ThenBy(r => r.Name)
            .AsNoTracking()
            .ToListAsync();
        return recipes.Select(MapRecipe).ToList();
    }

    public async Task<RecipeResponse?> GetRecipeByIdAsync(int id)
    {
        var r = await db.Recipes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && r.UserId == DefaultUserId);
        return r is null ? null : MapRecipe(r);
    }

    public async Task<RecipeResponse> CreateRecipeAsync(CreateRecipeRequest req)
    {
        var recipe = new Recipe
        {
            UserId       = DefaultUserId,
            Name         = req.Name,
            Ingredients  = req.Ingredients,
            Instructions = req.Instructions,
            PrepMinutes  = req.PrepMinutes,
            CookMinutes  = req.CookMinutes,
            Servings     = req.Servings,
            Tags         = req.Tags,
            Rating       = req.Rating,
            ImageUrl     = req.ImageUrl,
            SourceUrl    = req.SourceUrl,
        };
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return MapRecipe(recipe);
    }

    public async Task<RecipeResponse?> UpdateRecipeAsync(int id, UpdateRecipeRequest req)
    {
        var r = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == DefaultUserId);
        if (r is null) return null;
        if (req.Name         is not null) r.Name         = req.Name;
        if (req.Ingredients  is not null) r.Ingredients  = req.Ingredients;
        if (req.Instructions is not null) r.Instructions = req.Instructions;
        if (req.PrepMinutes  is not null) r.PrepMinutes  = req.PrepMinutes;
        if (req.CookMinutes  is not null) r.CookMinutes  = req.CookMinutes;
        if (req.Servings     is not null) r.Servings     = req.Servings;
        if (req.Tags         is not null) r.Tags         = req.Tags;
        if (req.Rating       is not null) r.Rating       = req.Rating;
        if (req.ImageUrl     is not null) r.ImageUrl     = req.ImageUrl;
        if (req.SourceUrl    is not null) r.SourceUrl    = req.SourceUrl;
        r.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapRecipe(r);
    }

    public async Task<bool> DeleteRecipeAsync(int id)
    {
        var r = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == DefaultUserId);
        if (r is null) return false;
        r.DeletedAt = DateTime.UtcNow;
        r.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<RecipeResponse?> ToggleFavoriteAsync(int id)
    {
        var r = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == DefaultUserId);
        if (r is null) return null;
        r.IsFavorite = !r.IsFavorite;
        r.UpdatedAt  = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapRecipe(r);
    }

    public async Task<WeekPlanResponse> GetWeekPlanAsync(string? week = null)
    {
        var (weekStart, weekEnd) = ParseWeek(week);
        var plans = await db.MealPlans
            .Include(m => m.Recipe)
            .Where(m => m.UserId == DefaultUserId && m.Date >= weekStart && m.Date < weekEnd)
            .OrderBy(m => m.Date).ThenBy(m => m.Slot)
            .AsNoTracking()
            .ToListAsync();

        var slots = plans.Select(m => new MealSlotResponse(
            m.Id, m.Date.ToString("yyyy-MM-dd"), m.Slot,
            m.RecipeId, m.Recipe?.Name, m.FreeText)).ToList();

        return new WeekPlanResponse(weekStart.ToString("yyyy-MM-dd"), slots);
    }

    public async Task<MealSlotResponse> SetMealSlotAsync(SetMealSlotRequest req)
    {
        if (!DateOnly.TryParseExact(req.Date, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var date))
            date = DateOnly.FromDateTime(DateTime.UtcNow);

        var existing = await db.MealPlans
            .Include(m => m.Recipe)
            .FirstOrDefaultAsync(m => m.UserId == DefaultUserId && m.Date == date && m.Slot == req.Slot);

        if (existing is not null)
        {
            existing.RecipeId  = req.RecipeId;
            existing.FreeText  = req.FreeText;
            existing.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return new MealSlotResponse(existing.Id, date.ToString("yyyy-MM-dd"), existing.Slot,
                existing.RecipeId, existing.Recipe?.Name, existing.FreeText);
        }

        var plan = new MealPlan
        {
            UserId   = DefaultUserId,
            Date     = date,
            Slot     = req.Slot,
            RecipeId = req.RecipeId,
            FreeText = req.FreeText,
        };
        db.MealPlans.Add(plan);
        await db.SaveChangesAsync();
        var recipeName = req.RecipeId.HasValue
            ? (await db.Recipes.FindAsync(req.RecipeId))?.Name
            : null;
        return new MealSlotResponse(plan.Id, date.ToString("yyyy-MM-dd"), plan.Slot, plan.RecipeId, recipeName, plan.FreeText);
    }

    public async Task<bool> DeleteMealSlotAsync(int id)
    {
        var m = await db.MealPlans.FirstOrDefaultAsync(m => m.Id == id && m.UserId == DefaultUserId);
        if (m is null) return false;
        db.MealPlans.Remove(m);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<int> ClearWeekAsync(string? week = null)
    {
        var (weekStart, weekEnd) = ParseWeek(week);
        var plans = await db.MealPlans
            .Where(m => m.UserId == DefaultUserId && m.Date >= weekStart && m.Date < weekEnd)
            .ToListAsync();
        db.MealPlans.RemoveRange(plans);
        await db.SaveChangesAsync();
        return plans.Count;
    }

    private static RecipeResponse MapRecipe(Recipe r) =>
        new(r.Id, r.Name, r.Ingredients, r.Instructions, r.PrepMinutes, r.CookMinutes,
            r.Servings, r.Tags, r.Rating, r.IsFavorite, r.ImageUrl, r.SourceUrl);

    private static (DateOnly start, DateOnly end) ParseWeek(string? week)
    {
        var today  = DateOnly.FromDateTime(DateTime.UtcNow);
        var monday = today.AddDays(-(int)today.DayOfWeek + (today.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
        return (monday, monday.AddDays(7));
    }
}
