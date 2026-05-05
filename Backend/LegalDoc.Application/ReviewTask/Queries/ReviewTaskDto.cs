using LegalDoc.Domain.Enums;

namespace LegalDoc.Application.ReviewTask.Queries;

public record ReviewTaskDto(Guid Id, Guid DocumentId, Guid LawyerId, string? Description, string Status, DateTime AssignedAt);