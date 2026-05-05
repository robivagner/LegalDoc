using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.ReviewTask.Commands;

public class UpdateReviewTaskLawyerCommandHandler(IReviewTaskRepository reviewTaskRepository, ILawyerRepository lawyerRepository) : IRequestHandler<UpdateReviewTaskLawyerCommand>
{
    public async Task Handle(UpdateReviewTaskLawyerCommand request, CancellationToken cancellationToken)
    {
        var reviewTask = await reviewTaskRepository.FindAsync(request.ReviewTaskId, cancellationToken);
        if (reviewTask is null) 
            throw new Exception("Review task not found.");
        
        var oldLawyer = await lawyerRepository.FindAsync(reviewTask.LawyerId, cancellationToken);
        if (oldLawyer is null)
            throw new Exception("Old lawyer not found.");
        if (oldLawyer.IsActive)
            throw new Exception("Active lawyer cannot be replaced.");
        
        var newLawyer = await lawyerRepository.FindAsync(request.LawyerId, cancellationToken);
        if (newLawyer is null)
            throw new Exception("New lawyer not found.");
        if (!newLawyer.IsActive)
            throw new Exception("New lawyer is not active.");
        
        reviewTask.UpdateLawyer(newLawyer.Id);
        
        await reviewTaskRepository.UpdateAsync(reviewTask, cancellationToken);
    }   
}