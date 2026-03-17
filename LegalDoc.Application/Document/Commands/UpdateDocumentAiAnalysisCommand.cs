using MediatR;

namespace LegalDoc.Application.Documents.Commands;

public record UpdateDocumentAiAnalysisCommand(Guid DocumentId, string Summary, string Clauses, string Risks) : IRequest;