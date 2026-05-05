using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public record UpdateReviewTaskLawyerCommand(Guid ReviewTaskId, Guid LawyerId) : IRequest;