namespace LegalDoc.Application.Documents.Queries;

public record DocumentDto(Guid Id, string Title, string FileName, string Status, DateTime UploadedAt);