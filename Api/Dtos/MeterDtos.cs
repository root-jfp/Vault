namespace Vault.Api.Dtos;

public sealed record MeterResponse(
    int Id,
    string Name,
    string MeterType,
    string Unit,
    decimal TariffRate,
    decimal? LatestReading,
    string? LatestReadingDate,
    decimal? MonthlyUsage,
    decimal? VsLastMonthPct,
    decimal? EstimatedBill
);

public sealed record MeterReadingResponse(
    int Id,
    string ReadingDate,
    decimal Value,
    decimal? Usage,
    string? Note
);

public sealed record CreateMeterRequest(
    string Name,
    string? MeterType,
    string? Unit,
    decimal? TariffRate
);

public sealed record UpdateMeterRequest(
    string? Name,
    decimal? TariffRate
);

public sealed record LogReadingRequest(
    string? ReadingDate,
    decimal Value,
    string? Note
);
