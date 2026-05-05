namespace LegalDoc.Application.Abstractions;

public interface IReviewTaskRepository
{
    Task AddAsync(Domain.Entities.ReviewTask task, CancellationToken cancellationToken = default);
    Task<Domain.Entities.ReviewTask?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.ReviewTask task, CancellationToken cancellationToken = default);
    IQueryable<Domain.Entities.ReviewTask> Query();
}