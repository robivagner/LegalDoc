namespace LegalDoc.Application.Document.Queries;

public record DocumentDto(
    Guid Id,
    Guid RegistryId,
    string Title,
    string FileName,
    string StoragePath,
    string Status,
    DateTime CreatedAt,
    string Content,
    string? Summary,
    string? Clauses,
    string? Risks);