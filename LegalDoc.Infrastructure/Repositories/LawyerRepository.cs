using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Entities;
using LegalDoc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalDoc.Infrastructure.Repositories;

public class LawyerRepository(AppDbContext dbContext) : ILawyerRepository
{
    public async Task AddAsync(Lawyer lawyer, CancellationToken cancellationToken = default)
    {
        await dbContext.Lawyers.AddAsync(lawyer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Lawyer?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Lawyers.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public IQueryable<Lawyer> Query()
    {
        return dbContext.Lawyers.AsQueryable();
    }
}