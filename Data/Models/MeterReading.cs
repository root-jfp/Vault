namespace Vault.Data.Models;

public sealed class MeterReading
{
    public int Id { get; init; }
    public int MeterId { get; set; }
    public int UserId { get; set; }
    public DateOnly ReadingDate { get; set; }
    public decimal Value { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public Meter Meter { get; init; } = null!;
}
