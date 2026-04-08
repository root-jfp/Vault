namespace Vault.Data.Models;

public sealed class TaskItem
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public int ColumnId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Priority { get; set; }   // "high", "med", "low"
    public string? Project { get; set; }
    public DateOnly? DueDate { get; set; }
    public int Position { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public TaskColumn Column { get; init; } = null!;
    public UserProfile User { get; init; } = null!;
}
