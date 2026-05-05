using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Queries;

public record GetReviewTasksQuery(Guid? LawyerId = null, ReviewTaskStatus? Status = null) : IRequest<List<ReviewTaskDto>>;