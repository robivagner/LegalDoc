using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Entities;
using LegalDoc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalDoc.Infrastructure.Repositories;

public class DocumentsRepository(AppDbContext dbContext) : IDocumentsRepository
{
    public async Task AddAsync(LegalDocument document, CancellationToken cancellationToken = default)
    {
        await dbContext.LegalDocuments.AddAsync(document, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<LegalDocument?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.LegalDocuments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(LegalDocument document, CancellationToken cancellationToken = default)
    {
        dbContext.LegalDocuments.Update(document);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<LegalDocument> Query()
    {
        return dbContext.LegalDocuments.AsQueryable();
    }
}