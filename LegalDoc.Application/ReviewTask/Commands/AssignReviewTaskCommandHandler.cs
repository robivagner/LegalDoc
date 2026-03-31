using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Enums;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public class AssignReviewTaskCommandHandler(IDocumentsRepository documentsRepository, ILawyerRepository lawyerRepository, IReviewTaskRepository reviewTaskRepository) : IRequestHandler<AssignReviewTaskCommand, Guid>
{
    public async Task<Guid> Handle(AssignReviewTaskCommand request, CancellationToken cancellationToken)
    {
        var document = await documentsRepository.FindAsync(request.DocumentId, cancellationToken);
        if (document == null)
            throw new Exception("Document not found.");
        if (document.Status != DocumentStatus.AwaitingReview)
            throw new InvalidOperationException("Document is not awaiting review.");
        
        document.MarkAsInReview();
        
        var lawyer = await lawyerRepository.FindAsync(request.LawyerId, cancellationToken);
        if (lawyer == null)
            throw new Exception("Lawyer not found.");
        if(!lawyer.IsActive)
            throw new InvalidOperationException("Lawyer is not active.");
        
        var task = Domain.Entities.ReviewTask.Create(request.DocumentId, request.LawyerId, request.Description);
        await reviewTaskRepository.AddAsync(task, cancellationToken);

        return task.Id;
    }
}