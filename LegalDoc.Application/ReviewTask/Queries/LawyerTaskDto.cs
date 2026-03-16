namespace LegalDoc.Application.ReviewTask.Queries;

public record LawyerTaskDto(Guid Id, Guid DocumentId, string TaskType, string Status, string? Description, DateTime AssignedAt);