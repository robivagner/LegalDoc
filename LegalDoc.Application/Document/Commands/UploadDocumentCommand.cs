using MediatR;

namespace LegalDoc.Application.Document.Commands;

public record UploadDocumentCommand(string Title, string FileName, byte[] FileContent, Guid RegistryId) : IRequest<Guid>;