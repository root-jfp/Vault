namespace Vault.Data.Models;

public sealed class MealPlan
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public DateOnly Date { get; set; }
    public string Slot { get; set; } = "lunch";            // breakfast, lunch, dinner
    public int? RecipeId { get; set; }
    public string? FreeText { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Recipe? Recipe { get; init; }
    public UserProfile User { get; init; } = null!;
}
