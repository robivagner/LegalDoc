using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public class UpdateReviewTaskStatusCommandHandler(IReviewTaskRepository reviewTaskRepository, IDocumentsRepository documentsRepository) : IRequestHandler<UpdateReviewTaskStatusCommand>
{
    public async Task Handle(UpdateReviewTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var reviewTask = await reviewTaskRepository.FindAsync(request.ReviewTaskId, cancellationToken);
        if (reviewTask is null) 
            throw new Exception("Review task not found.");

        reviewTask.UpdateStatus(request.Status);
        
        await reviewTaskRepository.UpdateAsync(reviewTask, cancellationToken);
        
        var document = await documentsRepository.FindAsync(reviewTask.DocumentId, cancellationToken);
        if (document is null)
            throw new Exception("Document not found.");
        
        if(request.Status == ReviewTaskStatus.Completed)
            document.MarkAsCompleted();
        else if (request.Status == ReviewTaskStatus.Reviewing)
            document.MarkAsInReview();
        else if (request.Status == ReviewTaskStatus.Rejected)
            document.MarkAsRejected();
        
        await documentsRepository.UpdateAsync(document, cancellationToken);
    }
}