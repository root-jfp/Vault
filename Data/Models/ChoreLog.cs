namespace Vault.Data.Models;

public sealed class ChoreLog
{
    public int Id { get; init; }
    public int ChoreId { get; set; }
    public int UserId { get; set; }
    public string CompletedBy { get; set; } = "José";
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
    public bool WasSwapped { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public Chore Chore { get; init; } = null!;
}
