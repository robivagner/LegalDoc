using MediatR;

namespace LegalDoc.Application.Registry.Queries;

public record GetRegistriesQuery() : IRequest<List<RegistryDto>>;