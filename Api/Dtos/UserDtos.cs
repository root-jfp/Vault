namespace Vault.Api.Dtos;

public sealed record UserResponse(
    int Id,
    string Name,
    string? AvatarColor
);

public sealed record CreateUserRequest(
    string Name,
    string? AvatarColor
);

public sealed record UpdateUserRequest(
    string? Name,
    string? AvatarColor
);
