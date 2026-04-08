namespace Vault.Api.Dtos;

public sealed record BudgetCategoryResponse(
    int Id,
    string Name,
    decimal MonthlyLimit,
    string? Color,
    string? Icon,
    decimal Spent,
    decimal Remaining,
    int Pct,
    bool IsOver
);

public sealed record TransactionResponse(
    int Id,
    int CategoryId,
    string CategoryName,
    decimal Amount,
    string? Description,
    string? Payee,
    string Date,
    bool IsRecurring
);

public sealed record BudgetSummaryResponse(
    string Month,
    decimal TotalBudget,
    decimal TotalSpent,
    decimal TotalRemaining,
    int SavingsRatePct,
    IReadOnlyList<BudgetCategoryResponse> Categories
);

public sealed record CreateCategoryRequest(
    string Name,
    decimal MonthlyLimit,
    string? Color,
    string? Icon
);

public sealed record UpdateCategoryRequest(
    string? Name,
    decimal? MonthlyLimit,
    string? Color,
    string? Icon
);

public sealed record CreateTransactionRequest(
    int CategoryId,
    decimal Amount,
    string? Description,
    string? Payee,
    string? Date,
    bool? IsRecurring
);

public sealed record UpdateTransactionRequest(
    int? CategoryId,
    decimal? Amount,
    string? Description,
    string? Payee,
    string? Date,
    bool? IsRecurring
);
