namespace Vault.Data.Models;

public sealed class CalendarEvent
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Title { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public string? LinkedModule { get; set; }              // task, maintenance, habit, birthday, manual
    public int? LinkedId { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceRule { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
}
