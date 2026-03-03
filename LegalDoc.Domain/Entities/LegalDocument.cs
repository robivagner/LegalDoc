using LegalDoc.Domain.Enums;

namespace LegalDoc.Domain.Entities;

public class LegalDocument
{
    private LegalDocument()
    {
        
    }

    public static LegalDocument Create(string title, string fileName, string storagePath)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title));
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        return new LegalDocument
        {
            Title = title,
            FileName = fileName,
            StoragePath = storagePath,
            Status = DocumentStatus.Uploaded,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string StoragePath { get; private set; } = null!;
    public string? Summary { get; private set; }
    public DocumentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public void UpdateSummary(string summary)
    {
        Summary = summary;
        Status = DocumentStatus.Completed;
    }
}