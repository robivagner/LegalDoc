using MediatR;

namespace LegalDoc.Application.Lawyer.Commands;

public record UpdateLawyerActivityCommand(Guid LawyerId, bool IsActive) : IRequest;