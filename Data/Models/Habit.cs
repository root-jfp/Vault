namespace Vault.Data.Models;

public sealed class Habit
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }          // "boolean" or "quant"
    public required string Frequency { get; set; }
    public required string Color { get; set; }         // CSS var: "var(--ac)"
    public required string HexColor { get; set; }      // Hex: "#1D9E75"
    public int WeeklyGoal { get; set; } = 7;
    public double Goal { get; set; }
    public string? UnitOfMeasure { get; set; }
    public int Increment { get; set; } = 1;
    public string? Icon { get; set; }
    public string? Notes { get; set; }
    public string DaysOfWeek { get; set; } = "0,1,2,3,4,5,6";
    public DateTime? StartDate { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
    public ICollection<HabitEntry> Entries { get; init; } = [];
}
