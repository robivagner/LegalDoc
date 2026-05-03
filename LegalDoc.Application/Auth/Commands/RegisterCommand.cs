using MediatR;

namespace LegalDoc.Application.Auth.Commands;

public record RegisterCommand(string UserName, string Email, string Password) : IRequest<Unit>;