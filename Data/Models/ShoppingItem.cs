namespace Vault.Data.Models;

public sealed class ShoppingItem
{
    public int Id { get; init; }
    public int ListId { get; set; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
    public string? Category { get; set; }
    public string? Note { get; set; }
    public bool IsDone { get; set; }
    public bool IsRecurring { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public ShoppingList List { get; init; } = null!;
    public UserProfile User { get; init; } = null!;
}
