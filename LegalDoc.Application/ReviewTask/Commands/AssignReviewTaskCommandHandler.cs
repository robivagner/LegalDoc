using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public class AssignReviewTaskCommandHandler(IDocumentsRepository documentsRepository, ILawyerRepository lawyerRepository, IReviewTaskRepository reviewTaskRepository) : IRequestHandler<AssignReviewTaskCommand, Guid>
{
    public async Task<Guid> Handle(AssignReviewTaskCommand request, CancellationToken cancellationToken)
    {
        var document = await documentsRepository.FindAsync(request.DocumentId, cancellationToken);
        if (document == null)
        {
            throw new Exception("Document not found.");
        }
        
        var lawyer = await lawyerRepository.FindAsync(request.LawyerId, cancellationToken);
        if (lawyer == null)
        {
            throw new Exception("Lawyer not found.");
        }
        
        var task = Domain.Entities.ReviewTask.Create(request.DocumentId, request.LawyerId, request.TaskType, request.Description);
        await reviewTaskRepository.AddAsync(task, cancellationToken);

        return task.Id;
    }
}