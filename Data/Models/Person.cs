namespace Vault.Data.Models;

public sealed class Person
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public DateOnly Date { get; set; }
    public string EventType { get; set; } = "birthday";   // birthday, anniversary, other
    public string? Category { get; set; }                  // family, friends, work
    public string? Notes { get; set; }
    public string? ReminderDays { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
}
