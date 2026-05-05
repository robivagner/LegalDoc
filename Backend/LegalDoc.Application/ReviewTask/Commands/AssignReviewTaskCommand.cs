using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public record AssignReviewTaskCommand(Guid DocumentId, Guid LawyerId, string Description) : IRequest<Guid>;