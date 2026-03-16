namespace LegalDoc.Application.Lawyer.Queries;

public record LawyerDto(Guid Id, string Name, string BarNumber, string? Email);