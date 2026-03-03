using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.Documents.Queries;

public record GetDocumentQuery(DocumentStatus? Status) : IRequest<List<DocumentDto>>;