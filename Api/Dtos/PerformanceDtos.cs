namespace Vault.Api.Dtos;

public sealed record HabitPerformanceResponse(
    int HabitId,
    string HabitName,
    string Color,
    int CompletionPct,
    int Streak,
    int DaysLogged,
    int TotalDays
);

public sealed record TaskPerformanceResponse(
    int TotalCompleted,
    int CompletedThisPeriod,
    int OpenTasks,
    int OverdueTasks,
    double VelocityPerWeek
);

public sealed record MaintenancePerformanceResponse(
    int TotalItems,
    int OnTimeCount,
    int OverdueCount,
    int OnTimePct
);

public sealed record PerformanceSummaryResponse(
    string Period,
    int ProductivityScore,
    int HabitCompletionPct,
    int TaskVelocity,
    int MaintenanceOnTimePct,
    int VsLastPeriodPts,
    IReadOnlyList<HabitPerformanceResponse> Habits,
    TaskPerformanceResponse Tasks,
    MaintenancePerformanceResponse Maintenance
);
