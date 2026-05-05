using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Registry.Commands;

public class CreateRegistryCommandHandler(IRegistryRepository repository) 
    : IRequestHandler<CreateRegistryCommand, Guid>
{
    public async Task<Guid> Handle(CreateRegistryCommand request, CancellationToken cancellationToken)
    {
        var registry = Domain.Entities.Registry.Create(request.Name, request.Location, request.Capacity);
        await repository.AddAsync(registry, cancellationToken);
        return registry.Id;
    }
}