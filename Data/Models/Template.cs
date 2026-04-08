namespace Vault.Data.Models;

public sealed class Template
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "habits";      // habits, tasks, projects, maintenance, routines
    public string Payload { get; set; } = "{}";            // JSON
    public string? Icon { get; set; }
    public bool IsBuiltIn { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
