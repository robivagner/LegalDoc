using MediatR;

namespace LegalDoc.Application.Document.Commands;

public record UploadDocumentCommand(string Title, string FileName, string StoragePath, string Content, Guid RegistryId) : IRequest<Guid>;