using LegalDoc.Domain.Entities;

namespace LegalDoc.Application.Abstractions;

public interface IDocumentsRepository
{
    Task AddAsync(LegalDocument document, CancellationToken cancellationToken = default);
    Task<LegalDocument?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<LegalDocument> Query();
}