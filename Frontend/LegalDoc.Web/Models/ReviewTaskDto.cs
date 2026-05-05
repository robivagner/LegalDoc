namespace LegalDoc.Web.Models;

public record ReviewTaskDto(Guid Id, Guid DocumentId, Guid LawyerId, string? Description, string Status, DateTime AssignedAt);