namespace LegalDoc.Web.Models;

public record DocumentDto(
    Guid Id,
    Guid RegistryId,
    string Title,
    string FileName,
    string StoragePath,
    string Status,
    DateTime CreatedAt,
    string Content,
    string? Summary,
    string? Clauses,
    string? Risks);

public record LawyerDto(Guid Id, string Name, string BarNumber, string? Email, bool IsActive);

public class RegistryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Availability { get; set; }
}

public record ReviewTaskDto(Guid Id, Guid DocumentId, Guid LawyerId, string? Description, string Status, DateTime AssignedAt);