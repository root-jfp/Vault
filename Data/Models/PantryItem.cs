namespace Vault.Data.Models;

public sealed class PantryItem
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string Category { get; set; } = "Other";
    public string Location { get; set; } = "pantry";      // fridge, freezer, pantry, other
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal? MinStock { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
}
