namespace Vault.Data.Models;

public sealed class Project
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "active";       // active, paused, done, archived
    public string? Color { get; set; }
    public DateOnly? Deadline { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
    public ICollection<TaskItem> Tasks { get; init; } = [];
}
