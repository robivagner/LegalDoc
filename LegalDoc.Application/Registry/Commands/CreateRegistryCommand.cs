using MediatR;

namespace LegalDoc.Application.Registry.Commands;

public record CreateRegistryCommand(string Name, string Location, int Capacity) : IRequest<Guid>;