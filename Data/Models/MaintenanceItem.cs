namespace Vault.Data.Models;

public sealed class MaintenanceItem
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string? Category { get; set; }
    public string? Room { get; set; }
    public string? Icon { get; set; }
    public int IntervalDays { get; set; }
    public DateTime? LastCompletedAt { get; set; }
    public DateTime? NextDueAt { get; set; }
    public string? Notes { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
    public ICollection<MaintenanceLog> Logs { get; init; } = [];
}
