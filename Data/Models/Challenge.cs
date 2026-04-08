namespace Vault.Data.Models;

public sealed class Challenge
{
    public int Id { get; init; }
    public int CreatedByUserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string TargetModule { get; set; } = "habits";
    public int TargetCount { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile CreatedByUser { get; init; } = null!;
    public ICollection<ChallengeProgress> Progress { get; init; } = [];
}
