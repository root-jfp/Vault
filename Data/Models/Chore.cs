namespace Vault.Data.Models;

public sealed class Chore
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string AssignedTo { get; set; } = "José";
    public string Frequency { get; set; } = "weekly";     // daily, weekly, biweekly
    public int? DayOfWeek { get; set; }
    public int EffortPoints { get; set; } = 1;
    public string? Room { get; set; }
    public string RotationType { get; set; } = "none";    // none, alternating, weekly
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
    public ICollection<ChoreLog> Logs { get; init; } = [];
}
