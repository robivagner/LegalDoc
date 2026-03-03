using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Documents.Queries;

public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, List<DocumentDto>>
{
    private readonly IDocumentsRepository _repository;

    public GetDocumentQueryHandler(IDocumentsRepository repository)
    {
        _repository = repository;
    }
    
    public Task<List<DocumentDto>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var documents = _repository.Query();

        if (request.Status is not null)
        {
            documents = documents.Where(d => d.Status == request.Status);
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