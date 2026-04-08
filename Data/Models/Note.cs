namespace Vault.Data.Models;

public sealed class Note
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
    public string? Color { get; set; }
    public bool IsPinned { get; set; }
    public bool IsStarred { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public UserProfile User { get; init; } = null!;
}
