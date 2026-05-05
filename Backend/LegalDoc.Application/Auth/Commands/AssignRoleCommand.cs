using MediatR;

namespace LegalDoc.Application.Auth.Commands;

public record AssignRoleCommand(string UserId, string RoleName) : IRequest<Unit>;