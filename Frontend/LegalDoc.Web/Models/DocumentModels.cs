namespace LegalDoc.Web.Models;

public record LawyerDto(Guid Id, string Name, string BarNumber, string? Email, bool IsActive);