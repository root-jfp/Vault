namespace Vault.Api.Dtos;

public sealed record ShoppingListSummaryResponse(
    int Id,
    string Name,
    string? Color,
    bool IsDefault,
    int TotalItems,
    int DoneItems
);

public sealed record ShoppingItemResponse(
    int Id,
    int ListId,
    string Name,
    decimal? Quantity,
    string? Unit,
    string? Category,
    string? Note,
    bool IsDone,
    bool IsRecurring,
    int SortOrder
);

public sealed record ShoppingListDetailResponse(
    int Id,
    string Name,
    string? Color,
    bool IsDefault,
    int TotalItems,
    int DoneItems,
    IReadOnlyList<ShoppingItemResponse> Items
);

public sealed record CreateShoppingListRequest(
    string Name,
    string? Color,
    bool? IsDefault
);

public sealed record CreateShoppingItemRequest(
    string Name,
    decimal? Quantity,
    string? Unit,
    string? Category,
    string? Note,
    bool? IsRecurring
);

public sealed record UpdateShoppingItemRequest(
    string? Name,
    decimal? Quantity,
    string? Unit,
    string? Category,
    string? Note,
    bool? IsRecurring
);
