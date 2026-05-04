using MediatR;

namespace LegalDoc.Application.Auth.Queries;

public record GetUsersQuery(string? UserName = null) : IRequest<List<UserDto>>;