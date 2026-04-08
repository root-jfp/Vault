namespace Vault.Api.Dtos;

public sealed record TemplateResponse(
    int Id,
    string Name,
    string? Description,
    string Category,
    string? Icon,
    bool IsBuiltIn,
    string Payload
);

public sealed record CreateTemplateRequest(
    string Name,
    string? Description,
    string Category,
    string? Icon,
    string Payload
);
