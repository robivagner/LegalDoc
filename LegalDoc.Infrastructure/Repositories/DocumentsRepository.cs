using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Entities;
using LegalDoc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalDoc.Infrastructure.Repositories;

public class DocumentsRepository : IDocumentsRepository
{
    private readonly AppDbContext _context;

    public DocumentsRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(LegalDocument document, CancellationToken cancellationToken = default)
    {
        await _context.LegalDocuments.AddAsync(document, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<LegalDocument?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LegalDocuments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public IQueryable<LegalDocument> Query()
    {
        return _context.LegalDocuments.AsQueryable();
    }
}