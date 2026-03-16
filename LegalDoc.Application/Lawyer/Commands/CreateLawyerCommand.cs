using MediatR;

namespace LegalDoc.Application.Lawyer.Commands;

public record CreateLawyerCommand(string Name, string BarNumber, string? Email) : IRequest<Guid>;
