namespace Vault.Data.Models;

public sealed class XpEvent
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Action { get; set; }            // habit_done, task_done, chore_done, maintenance_done
    public int Points { get; set; }
    public string? LinkedModule { get; set; }
    public int? LinkedId { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public UserProfile User { get; init; } = null!;
}
