using MediatR;

namespace LegalDoc.Application.Document.Commands;

public record UpdateDocumentAiAnalysisCommand(Guid DocumentId) : IRequest;