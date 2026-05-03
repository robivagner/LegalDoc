using MediatR;

namespace LegalDoc.Application.Auth.Commands;

public record AssignRoleCommand(string UserName, string RoleName) : IRequest<Unit>;