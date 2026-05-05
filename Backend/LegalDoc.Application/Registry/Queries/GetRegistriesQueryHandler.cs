using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Registry.Queries;

public class GetRegistriesQueryHandler(IRegistryRepository repository) : IRequestHandler<GetRegistriesQuery, List<RegistryDto>>
{
    public Task<List<RegistryDto>> Handle(GetRegistriesQuery request, CancellationToken cancellationToken)
    {
        var result = repository.Query()
            .Select(r => new RegistryDto(r.Id, r.Name, r.Location, r.Capacity, r.Availability)).ToList();
        
        return Task.FromResult(result);
    }
}