using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public class UpdateReviewTaskStatusCommandHandler(IReviewTaskRepository reviewTaskRepository, IDocumentsRepository documentsRepository, ILawyerRepository lawyerRepository) : IRequestHandler<UpdateReviewTaskStatusCommand>
{
    public async Task Handle(UpdateReviewTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var reviewTask = await reviewTaskRepository.FindAsync(request.ReviewTaskId, cancellationToken);
        if (reviewTask is null) 
            throw new Exception("Review task not found.");
        
        var lawyer = await lawyerRepository.FindAsync(reviewTask.LawyerId, cancellationToken);
        
        if (lawyer is null)
            throw new Exception("Lawyer not found.");
        if (!lawyer.IsActive)
            throw new InvalidOperationException("Lawyer is not active.");

        reviewTask.UpdateStatus(request.Status);
        
        await reviewTaskRepository.UpdateAsync(reviewTask, cancellationToken);
        
        var document = await documentsRepository.FindAsync(reviewTask.DocumentId, cancellationToken);
        if (document is null)
            throw new Exception("Document not found.");
        
        if(request.Status == ReviewTaskStatus.Completed)
            document.MarkAsCompleted();
        else if (request.Status == ReviewTaskStatus.Rejected)
            document.MarkAsRejected();
        
        await documentsRepository.UpdateAsync(document, cancellationToken);
    }
}