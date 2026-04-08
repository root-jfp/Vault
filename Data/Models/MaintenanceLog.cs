namespace Vault.Data.Models;

public sealed class MaintenanceLog
{
    public int Id { get; init; }
    public int ItemId { get; set; }
    public int UserId { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
    public decimal? Cost { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public MaintenanceItem Item { get; init; } = null!;
}
