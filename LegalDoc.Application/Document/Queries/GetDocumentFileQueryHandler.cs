using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Document.Queries;

public class GetDocumentFileQueryHandler(
    IDocumentsRepository documentRepository,
    IFileStorageService fileStorageService) 
    : IRequestHandler<GetDocumentFileQuery, DocumentFileDto>
{
    public async Task<DocumentFileDto> Handle(GetDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var document = await documentRepository.FindAsync(request.DocumentId, cancellationToken);

        if (document == null) 
        {
            throw new KeyNotFoundException("Document negăsit.");
        }
        
        var fileBytes = await fileStorageService.ReadFileAsync(document.StoragePath, cancellationToken);

        return new DocumentFileDto(fileBytes, "application/pdf", document.FileName);
    }
}