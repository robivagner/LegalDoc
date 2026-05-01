using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Document.Commands;

public class UpdateDocumentAiAnalysisCommandHandler(IDocumentsRepository repository, IAiService aiService) : IRequestHandler<UpdateDocumentAiAnalysisCommand>
{
    public async Task Handle(UpdateDocumentAiAnalysisCommand request, CancellationToken cancellationToken)
    {
        var document = await repository.FindAsync(request.DocumentId, cancellationToken);

        if (document == null)
            throw new Exception("Document not found.");
        
        var analysis = await aiService.AnalyzeDocumentAsync(document.Content);
        
        if (analysis != null)
            document.UpdateAiAnalysis(analysis.Summary, analysis.Clauses, analysis.Risks);

        await repository.UpdateAsync(document, cancellationToken);
    }
}