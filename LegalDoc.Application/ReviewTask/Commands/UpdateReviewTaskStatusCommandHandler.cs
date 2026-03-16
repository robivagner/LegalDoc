using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public class UpdateReviewTaskStatusCommandHandler(IReviewTaskRepository repository) : IRequestHandler<UpdateReviewTaskStatusCommand>
{
    public async Task Handle(UpdateReviewTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var reviewTask = await repository.FindAsync(request.ReviewTaskId, cancellationToken);

        if (reviewTask is null)
        {
            throw new Exception("Review task not found.");
        }

        reviewTask.UpdateStatus(request.Status);
        
        await repository.UpdateAsync(reviewTask, cancellationToken);
    }
}