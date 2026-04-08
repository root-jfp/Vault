namespace Vault.Data.Models;

public sealed class BudgetCategory
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public decimal MonthlyLimit { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
    public ICollection<Transaction> Transactions { get; init; } = [];
}
