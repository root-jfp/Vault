namespace Vault.Api.Dtos;

public sealed record HabitResponse(
    int Id,
    string Name,
    string Color,
    string Hex,
    string Type,
    string Freq,
    int Streak,
    int Pct,
    bool Done,
    double Qty,
    double Target,
    string? Unit,
    int WeekDone,
    int WeekGoal,
    string? Icon,
    string? Notes,
    string HmView,
    string DaysOfWeek
);

public sealed record HeatmapCell(
    int Day,
    bool Empty,
    bool IsToday,
    bool Future,
    int Level
);

public sealed record HabitDetailResponse(
    int Id,
    string Name,
    string Type,
    string Freq,
    string Color,
    string? Icon,
    double Goal,
    string? Uom,
    int Increment,
    string? StartDate,
    int WeeklyGoal,
    string? Notes,
    string DaysOfWeek
);

public sealed record CreateHabitRequest(
    string Name,
    string Type,
    string Freq,
    string Color,
    string? Icon,
    double? Goal,
    string? Uom,
    int? Increment,
    string? StartDate,
    int? WeeklyGoal,
    string? Notes,
    string? DaysOfWeek
);

public sealed record UpdateHabitRequest(
    string? Name,
    string? Type,
    string? Freq,
    string? Color,
    string? Icon,
    double? Goal,
    string? Uom,
    int? Increment,
    string? StartDate,
    int? WeeklyGoal,
    string? Notes,
    string? DaysOfWeek
);

public sealed record ToggleHabitRequest(
    double? Amount
);
