using MediatR;

namespace LegalDoc.Application.Auth.Queries;

public record GetCurrentUserQuery(string UserId) : IRequest<UserDto>;