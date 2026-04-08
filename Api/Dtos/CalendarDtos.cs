namespace Vault.Api.Dtos;

public sealed record CalendarEventResponse(
    int Id,
    string Title,
    string Date,
    string? StartTime,
    string? EndTime,
    string? Color,
    string? Description,
    string? LinkedModule,
    int? LinkedId,
    bool IsRecurring,
    string Source          // manual, task, maintenance, birthday
);

public sealed record CreateCalendarEventRequest(
    string Title,
    string Date,
    string? StartTime,
    string? EndTime,
    string? Color,
    string? Description,
    bool? IsRecurring,
    string? RecurrenceRule
);

public sealed record UpdateCalendarEventRequest(
    string? Title,
    string? Date,
    string? StartTime,
    string? EndTime,
    string? Color,
    string? Description
);
