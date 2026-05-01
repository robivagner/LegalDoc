namespace LegalDoc.Application.Document.Queries;

public record DocumentDto(Guid Id, Guid RegistryId, string Title, string FileName, string Status, DateTime CreatedAt, string? Summary, string? Clauses, string? Risks);