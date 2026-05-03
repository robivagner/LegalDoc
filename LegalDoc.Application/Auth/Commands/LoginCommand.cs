using MediatR;

namespace LegalDoc.Application.Auth.Commands;

public record LoginCommand(string UserName, string Password) : IRequest<AuthResponse>;