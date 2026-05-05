
using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Queries;

public class GetReviewTasksQueryHandler(IReviewTaskRepository repository) : IRequestHandler<GetReviewTasksQuery, List<ReviewTaskDto>>
{
    public Task<List<ReviewTaskDto>> Handle(GetReviewTasksQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Query();
        
        if (request.LawyerId.HasValue)
        {
            query = query.Where(t => t.LawyerId == request.LawyerId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        var result = query.Select(t => new ReviewTaskDto(
                t.Id,
                t.DocumentId,
                t.LawyerId,
                t.Description,
                t.Status.ToString(),
                t.AssignedAt))
            .ToList();

        return Task.FromResult(result);
    }
}