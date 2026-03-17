namespace LegalDoc.Application.Documents.Queries;

public record DocumentDto(Guid Id, Guid RegistryId, string Title, string FileName, string Status, DateTime CreatedAt, string? Summary, string? Clauses, string? Risks);