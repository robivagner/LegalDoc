using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Documents.Commands;

public class UpdateDocumentAiAnalysisCommandHandler(IDocumentsRepository repository) : IRequestHandler<UpdateDocumentAiAnalysisCommand>
{
    public async Task Handle(UpdateDocumentAiAnalysisCommand request, CancellationToken cancellationToken)
    {
        var document = await repository.FindAsync(request.DocumentId, cancellationToken);

        if (document == null)
            throw new Exception("Document not found.");
        
        document.UpdateAiAnalysis(request.Summary, request.Clauses, request.Risks);

        await repository.UpdateAsync(document, cancellationToken);
    }
}