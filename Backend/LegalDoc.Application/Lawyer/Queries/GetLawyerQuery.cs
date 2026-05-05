using MediatR;

namespace LegalDoc.Application.Lawyer.Queries;

public record GetLawyerQuery(Guid Id) : IRequest<Domain.Entities.Lawyer?>;