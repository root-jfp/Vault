namespace Vault.Api.Dtos;

public sealed record NoteResponse(
    int Id,
    string Title,
    string? Content,
    string? Color,
    bool IsPinned,
    bool IsStarred,
    string? Tags,
    string UpdatedAt
);

public sealed record CreateNoteRequest(
    string Title,
    string? Content,
    string? Color,
    bool? IsPinned,
    bool? IsStarred,
    string? Tags
);

public sealed record UpdateNoteRequest(
    string? Title,
    string? Content,
    string? Color,
    bool? IsPinned,
    bool? IsStarred,
    string? Tags
);
