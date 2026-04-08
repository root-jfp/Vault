namespace Vault.Data.Models;

public sealed class UserProfile
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? AvatarColor { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}
