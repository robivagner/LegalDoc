using MediatR;

namespace LegalDoc.Application.Lawyer.Commands;

public record CreateLawyerCommand(Guid Id, string Name, string BarNumber, string Email) : IRequest<Guid>;
