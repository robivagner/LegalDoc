
namespace LegalDoc.Application.Abstractions;

public interface IRegistryRepository
{
    Task AddAsync(Domain.Entities.Registry registry, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Registry?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<Domain.Entities.Registry> Query();
}