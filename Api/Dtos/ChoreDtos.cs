namespace Vault.Api.Dtos;

public sealed record ChoreResponse(
    int Id,
    string Name,
    string AssignedTo,
    string Frequency,
    int? DayOfWeek,
    int EffortPoints,
    string? Room,
    string RotationType,
    bool IsDoneThisWeek,
    string? CompletedBy
);

public sealed record ChoreWeekSummaryResponse(
    string Week,
    int JoseScore,
    int AnaScore,
    int FairnessPct,
    IReadOnlyList<ChoreResponse> Chores
);

public sealed record CreateChoreRequest(
    string Name,
    string? AssignedTo,
    string? Frequency,
    int? DayOfWeek,
    int? EffortPoints,
    string? Room,
    string? RotationType
);

public sealed record UpdateChoreRequest(
    string? Name,
    string? AssignedTo,
    string? Frequency,
    int? DayOfWeek,
    int? EffortPoints,
    string? Room,
    string? RotationType
);

public sealed record CompleteChoreRequest(
    string? CompletedBy,
    string? Note
);

public sealed record SwapChoreRequest(
    string NewAssignee
);
