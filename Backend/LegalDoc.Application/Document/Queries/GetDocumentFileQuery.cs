using MediatR;

namespace LegalDoc.Application.Document.Queries;

public record GetDocumentFileQuery(Guid DocumentId) : IRequest<DocumentFileDto>;