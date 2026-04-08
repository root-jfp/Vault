namespace Vault.Api.Dtos;

public sealed record ProjectResponse(
    int Id,
    string Name,
    string? Description,
    string Status,
    string? Color,
    string? Deadline,
    int TotalTasks,
    int OpenTasks,
    int DoneTasks,
    int Pct,
    int? DaysLeft
);

public sealed record CreateProjectRequest(
    string Name,
    string? Description,
    string? Status,
    string? Color,
    string? Deadline
);

public sealed record UpdateProjectRequest(
    string? Name,
    string? Description,
    string? Status,
    string? Color,
    string? Deadline
);
