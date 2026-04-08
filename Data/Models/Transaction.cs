namespace Vault.Data.Models;

public sealed class Transaction
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Payee { get; set; }
    public DateOnly Date { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceRule { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public BudgetCategory Category { get; init; } = null!;
    public UserProfile User { get; init; } = null!;
}
