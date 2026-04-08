namespace Vault.Api.Dtos;

public sealed record PantryItemResponse(
    int Id,
    string Name,
    string Category,
    string Location,
    decimal Quantity,
    string? Unit,
    decimal? MinStock,
    string? ExpiryDate,
    string? Note,
    string StockStatus       // out, low, expiring, ok
);

public sealed record PantryStatsResponse(
    int TotalItems,
    int LowCount,
    int ExpiringCount,
    int OutCount
);

public sealed record CreatePantryItemRequest(
    string Name,
    string? Category,
    string? Location,
    decimal Quantity,
    string? Unit,
    decimal? MinStock,
    string? ExpiryDate,
    string? Note
);

public sealed record UpdatePantryItemRequest(
    string? Name,
    string? Category,
    string? Location,
    decimal? Quantity,
    string? Unit,
    decimal? MinStock,
    string? ExpiryDate,
    string? Note
);

public sealed record AdjustPantryRequest(
    decimal Delta
);
