namespace Vault.Api.Dtos;

public sealed record HealthMetricResponse(
    int Id,
    string MetricType,
    string Date,
    double Value,
    string Unit,
    string? Note
);

public sealed record HealthSummaryResponse(
    string MetricType,
    double TodayValue,
    double GoalValue,
    int Pct,
    int Streak,
    double Avg7d,
    double Avg30d,
    string Unit
);

public sealed record LogHealthRequest(
    string MetricType,
    string? Date,        // defaults to today
    double Value,
    string? Unit,
    string? Note
);

public sealed record UpdateHealthRequest(
    double? Value,
    string? Unit,
    string? Note
);

public sealed record QuickAddWaterRequest(
    double Amount   // in ml
);
