using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Entities;
using LegalDoc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalDoc.Infrastructure.Repositories;

public class RegistryRepository(AppDbContext dbContext) : IRegistryRepository
{
    public async Task AddAsync(Registry registry, CancellationToken cancellationToken = default)
    {
        await dbContext.Registries.AddAsync(registry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Registry?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Registries.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public IQueryable<Registry> Query()
    {
        return dbContext.Registries.AsQueryable();
    }
}