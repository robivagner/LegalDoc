using System.Diagnostics.CodeAnalysis;
using LegalDoc.Domain.Enums;

namespace LegalDoc.Domain.Entities;

public class LegalDocument
{
    private LegalDocument()
    {
        
    }
    
    public Guid Id { get; private set; }
    public Guid RegistryId { get; private set; }
    public string Title { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string StoragePath { get; private set; } = null!;
    public string? Summary { get; private set; }
    public string? Clauses { get; private set; }
    public string? Risks { get; private set; }
    public DocumentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static LegalDocument Create(string title, string fileName, string storagePath, Guid registryId)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentException("Document title cannot be null or empty.", nameof(title));
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("Document file name cannot be null or empty.", nameof(fileName));
        }

        return new LegalDocument
        {
            Id = Guid.NewGuid(),
            Title = title,
            FileName = fileName,
            StoragePath = storagePath,
            Status = DocumentStatus.Uploaded,
            CreatedAt = DateTime.UtcNow,
            RegistryId =  registryId
        };
    }
    
    public void UpdateAiAnalysis(string summary, string clauses, string risks)
    {
        if (Status == DocumentStatus.InReview ||  Status == DocumentStatus.Completed)
            throw new InvalidOperationException("Cannot update analysis in review or when completed.");
        
        Summary = summary;
        Clauses = clauses;
        Risks = risks;
        Status = DocumentStatus.AwaitingReview;
    }
    
    public void MarkAsInReview()
    {
        Status = DocumentStatus.InReview;
    }

    public void MarkAsCompleted()
    {
        Status = DocumentStatus.Completed;
    }

    public void MarkAsRejected()
    {
        Status = DocumentStatus.Rejected;
    }
}