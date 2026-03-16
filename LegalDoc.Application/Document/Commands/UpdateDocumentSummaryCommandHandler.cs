using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Documents.Commands;

public class UpdateDocumentSummaryCommandHandler(IDocumentsRepository repository) : IRequestHandler<UpdateDocumentSummaryCommand>
{
    public async Task Handle(UpdateDocumentSummaryCommand request, CancellationToken cancellationToken)
    {
        var document = await repository.FindAsync(request.DocumentId, cancellationToken);

        if (document == null)
        {
            throw new Exception("Document not found.");
        }
        
        document.UpdateSummary(request.Summary);

        await repository.UpdateAsync(document, cancellationToken);
    }
}