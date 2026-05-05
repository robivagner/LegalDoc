using LegalDoc.Application.Auth.Queries;
using MediatR;

namespace LegalDoc.Application.Auth.Commands;

public record ChangeUsernameCommand(Guid UserId, string NewUsername) : IRequest<AuthResponse>;