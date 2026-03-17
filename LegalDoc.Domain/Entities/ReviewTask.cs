using LegalDoc.Domain.Enums;

namespace LegalDoc.Domain.Entities;

public class ReviewTask
{
    public ReviewTask()
    {
        
    }
    
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public Guid LawyerId { get; private set; }
    public string? Description { get; private set; }
    public ReviewTaskStatus Status { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public static ReviewTask Create(Guid documentId, Guid lawyerId, string? description)
    {
        return new ReviewTask
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            LawyerId = lawyerId,
            Description = description,
            Status = ReviewTaskStatus.Created,
            AssignedAt = DateTime.UtcNow
        };
    }
    
    public void UpdateStatus(ReviewTaskStatus newStatus)
    {
        if(newStatus == ReviewTaskStatus.Created)
            throw new InvalidOperationException("Cannot update status of a review task to Created.");
        
        Status = newStatus;
    }
}