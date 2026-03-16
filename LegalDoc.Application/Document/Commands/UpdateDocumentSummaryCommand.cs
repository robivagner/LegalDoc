using MediatR;

namespace LegalDoc.Application.Documents.Commands;

public record UpdateDocumentSummaryCommand(Guid DocumentId, string Summary) : IRequest;