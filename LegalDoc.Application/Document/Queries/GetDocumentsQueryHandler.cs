using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Documents.Queries;

public class GetDocumentsQueryHandler(IDocumentsRepository repository)
    : IRequestHandler<GetDocumentsQuery, List<DocumentDto>>
{
    public Task<List<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = repository.Query();
        
        if (request.Status is not null)
        {
            documents = documents.Where(d => d.Status == request.Status);
        }
        
        if (request.RegistryId is not null)
        {
            documents = documents.Where(d => d.RegistryId == request.RegistryId);
        }

        var result = documents.Select(d => new DocumentDto(
            d.Id,
            d.Title,
            d.FileName,
            d.Status.ToString(),
            d.CreatedAt)).ToList();
    
        return Task.FromResult(result);
    }
}