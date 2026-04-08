namespace Vault.Data.Models;

public sealed class ChallengeProgress
{
    public int Id { get; init; }
    public int ChallengeId { get; set; }
    public int UserId { get; set; }
    public int CurrentCount { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Challenge Challenge { get; init; } = null!;
    public UserProfile User { get; init; } = null!;
}
