namespace Vault.Data.Models;

public sealed class Recipe
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string? Ingredients { get; set; }
    public string? Instructions { get; set; }
    public int? PrepMinutes { get; set; }
    public int? CookMinutes { get; set; }
    public int? Servings { get; set; }
    public string? Tags { get; set; }
    public int? Rating { get; set; }
    public bool IsFavorite { get; set; }
    public string? ImageUrl { get; set; }
    public string? SourceUrl { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
}
