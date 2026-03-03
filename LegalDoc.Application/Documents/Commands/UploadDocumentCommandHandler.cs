using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Entities;
using MediatR;

namespace LegalDoc.Application.Documents.Commands;

public sealed class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Guid>
{
    private readonly IDocumentsRepository _repository;

    public UploadDocumentCommandHandler(IDocumentsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = LegalDocument.Create(request.Title, request.FileName, request.StoragePath);
        await _repository.AddAsync(document, cancellationToken);
        return document.Id;
    }
}