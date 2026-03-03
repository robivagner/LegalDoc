using MediatR;

namespace LegalDoc.Application.Documents.Commands;

public record UploadDocumentCommand(string Title, string FileName, string StoragePath) : IRequest<Guid>;