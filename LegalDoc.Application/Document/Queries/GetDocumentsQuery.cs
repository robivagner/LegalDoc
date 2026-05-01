using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.Document.Queries;

public record GetDocumentsQuery(DocumentStatus? Status, Guid? RegistryId = null) : IRequest<List<DocumentDto>>;