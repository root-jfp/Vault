namespace Vault.Api.Dtos;

public sealed record RecipeResponse(
    int Id,
    string Name,
    string? Ingredients,
    string? Instructions,
    int? PrepMinutes,
    int? CookMinutes,
    int? Servings,
    string? Tags,
    int? Rating,
    bool IsFavorite,
    string? ImageUrl,
    string? SourceUrl
);

public sealed record MealSlotResponse(
    int? Id,
    string Date,
    string Slot,
    int? RecipeId,
    string? RecipeName,
    string? FreeText
);

public sealed record WeekPlanResponse(
    string WeekStart,
    IReadOnlyList<MealSlotResponse> Slots
);

public sealed record CreateRecipeRequest(
    string Name,
    string? Ingredients,
    string? Instructions,
    int? PrepMinutes,
    int? CookMinutes,
    int? Servings,
    string? Tags,
    int? Rating,
    string? ImageUrl,
    string? SourceUrl
);

public sealed record UpdateRecipeRequest(
    string? Name,
    string? Ingredients,
    string? Instructions,
    int? PrepMinutes,
    int? CookMinutes,
    int? Servings,
    string? Tags,
    int? Rating,
    string? ImageUrl,
    string? SourceUrl
);

public sealed record SetMealSlotRequest(
    string Date,
    string Slot,
    int? RecipeId,
    string? FreeText
);
