namespace Vault.Api.Dtos;

public sealed record TaskItemResponse(
    int Id,
    string Title,
    string? Description,
    string Priority,
    string? Project,
    string? Due,        // ISO date "2026-04-10" or null
    string ColName,
    int ColumnId,
    int Position
);

public sealed record TaskColumnResponse(
    int Id,
    string Name,
    string Color,
    int Position,
    List<TaskItemResponse> Tasks
);

public sealed record TaskDetailResponse(
    int Id,
    string Title,
    string? Description,
    string Priority,
    string? Project,
    string? Due,
    int ColumnId,
    string ColName,
    int Position
);

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    string? Priority,
    string? Project,
    string? Due,
    int? ColumnId
);

public sealed record UpdateTaskRequest(
    string? Title,
    string? Description,
    string? Priority,
    string? Project,
    string? Due,
    int? ColumnId
);

public sealed record MoveTaskRequest(
    int ColumnId,
    int Position
);
