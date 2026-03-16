using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Entities;
using MediatR;

namespace LegalDoc.Application.Documents.Commands;

public sealed class UploadDocumentCommandHandler(IDocumentsRepository documentRepository, IRegistryRepository registryRepository)
    : IRequestHandler<UploadDocumentCommand, Guid>
{
    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var registry = await registryRepository.FindAsync(request.RegistryId, cancellationToken);

        if (registry == null)
        {
            throw new Exception("Registry not found!");
        }
        
        registry.DocumentAdded();
        
        var document = LegalDocument.Create(request.Title, request.FileName, request.StoragePath, request.RegistryId);
        await documentRepository.AddAsync(document, cancellationToken);
        return document.Id;
    }
}