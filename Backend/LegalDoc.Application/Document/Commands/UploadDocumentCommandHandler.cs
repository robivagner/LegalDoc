using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Entities;
using MediatR;

namespace LegalDoc.Application.Document.Commands;

public sealed class UploadDocumentCommandHandler(
    IDocumentsRepository documentRepository,
    IRegistryRepository registryRepository,
    IDocumentTextExtractor documentTextExtractor,
    IFileStorageService fileStorageService)
    : IRequestHandler<UploadDocumentCommand, Guid>
{
    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var registry = await registryRepository.FindAsync(request.RegistryId, cancellationToken);

        if (registry == null)
        {
            throw new KeyNotFoundException("Registry not found!");
        }
        
        registry.DocumentAdded();
        
        var storagePath = await fileStorageService.SaveFileAsync(request.FileContent, request.FileName, cancellationToken);
        var content = documentTextExtractor.ExtractTextFromPdf(storagePath);
        
        var document = LegalDocument.Create(request.Title, request.FileName, storagePath, content, request.RegistryId);
        await documentRepository.AddAsync(document, cancellationToken);
        
        return document.Id;
    }
}