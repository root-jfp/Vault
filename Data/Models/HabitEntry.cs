namespace Vault.Data.Models;

public sealed class HabitEntry
{
    public int Id { get; init; }
    public int HabitId { get; init; }
    public int UserId { get; init; }
    public DateOnly Date { get; init; }
    public int Count { get; set; } = 1;
    public double Amount { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Habit Habit { get; init; } = null!;
}
