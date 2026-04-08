namespace Vault.Data.Models;

public sealed class HealthMetric
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string MetricType { get; set; }        // water, weight, sleep, hr
    public DateOnly Date { get; set; }
    public double Value { get; set; }
    public string Unit { get; set; } = "ml";
    public string? Note { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UserProfile User { get; init; } = null!;
}
