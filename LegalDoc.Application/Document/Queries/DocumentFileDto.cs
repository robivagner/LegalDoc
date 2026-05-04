namespace LegalDoc.Application.Document.Queries;

public record DocumentFileDto(byte[] Content, string ContentType, string FileName);