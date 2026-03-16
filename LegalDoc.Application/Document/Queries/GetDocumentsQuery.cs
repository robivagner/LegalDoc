using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.Documents.Queries;

public record GetDocumentsQuery(DocumentStatus? Status, Guid? RegistryId = null) : IRequest<List<DocumentDto>>;