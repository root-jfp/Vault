namespace Vault.Api.Dtos;

public sealed record MaintenanceItemResponse(
    int Id,
    string Name,
    string? Category,
    string? Room,
    string? Icon,
    int IntervalDays,
    string? LastCompletedAt,
    string? NextDueAt,
    string? Notes,
    string Status,          // overdue, soon, ok
    int DaysUntilDue,
    int OverduePercent
);

public sealed record MaintenanceLogResponse(
    int Id,
    string CompletedAt,
    string? Note,
    decimal? Cost
);

public sealed record MaintenanceItemDetailResponse(
    int Id,
    string Name,
    string? Category,
    string? Room,
    string? Icon,
    int IntervalDays,
    string? LastCompletedAt,
    string? NextDueAt,
    string? Notes,
    string Status,
    int DaysUntilDue,
    int OverduePercent,
    IReadOnlyList<MaintenanceLogResponse> Logs
);

public sealed record CreateMaintenanceItemRequest(
    string Name,
    string? Category,
    string? Room,
    string? Icon,
    int IntervalDays,
    string? Notes
);

public sealed record UpdateMaintenanceItemRequest(
    string? Name,
    string? Category,
    string? Room,
    string? Icon,
    int? IntervalDays,
    string? Notes
);

public sealed record CompleteMaintenanceRequest(
    string? Note,
    decimal? Cost
);
