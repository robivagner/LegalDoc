using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.Document.Queries;

public record GetDocumentsQuery(Guid? DocumentId = null, Guid? RegistryId = null, DocumentStatus? Status = null) : IRequest<List<DocumentDto>>;