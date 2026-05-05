using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Document.Queries;

public class GetDocumentsQueryHandler(IDocumentsRepository repository)
    : IRequestHandler<GetDocumentsQuery, List<DocumentDto>>
{
    public Task<List<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = repository.Query();

        if (request.DocumentId is not null)
        {
            documents = documents.Where(d => d.Id == request.DocumentId);
        }
        else
        {
            if (request.RegistryId is not null)
            {
                documents = documents.Where(d => d.RegistryId == request.RegistryId);
            }
        
            if (request.Status is not null)
            {
                documents = documents.Where(d => d.Status == request.Status);
            }
        }
        
        var result = documents.Select(d => new DocumentDto(
            d.Id,
            d.RegistryId,
            d.Title,
            d.FileName,
            d.StoragePath,
            d.Status.ToString(),
            d.CreatedAt,
            d.Content,
            d.Summary,
            d.Clauses,
            d.Risks)).ToList();
    
        return Task.FromResult(result);
    }
}