namespace Vault.Api.Dtos;

public sealed record PersonResponse(
    int Id,
    string Name,
    string Date,           // "YYYY-MM-DD"
    string EventType,
    string? Category,
    string? Notes,
    string? ReminderDays,
    int DaysUntil,
    int? NextAge,
    int? YearsTogether,
    bool IsMilestone
);

public sealed record CreatePersonRequest(
    string Name,
    string Date,
    string? EventType,
    string? Category,
    string? Notes,
    string? ReminderDays
);

public sealed record UpdatePersonRequest(
    string? Name,
    string? Date,
    string? EventType,
    string? Category,
    string? Notes,
    string? ReminderDays
);
