using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public record UpdateReviewTaskStatusCommand(Guid ReviewTaskId, ReviewTaskStatus Status) : IRequest;