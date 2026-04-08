namespace Vault.Data.Models;

public sealed class Meter
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string MeterType { get; set; } = "electricity"; // electricity, water, gas
    public string Unit { get; set; } = "kWh";
    public decimal TariffRate { get; set; }
    public decimal? CurrentReading { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
    public ICollection<MeterReading> Readings { get; init; } = [];
}
