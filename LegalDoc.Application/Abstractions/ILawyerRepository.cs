namespace LegalDoc.Application.Abstractions;

public interface ILawyerRepository
{
    Task AddAsync(Domain.Entities.Lawyer lawyer, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Lawyer?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.Lawyer lawyer, CancellationToken cancellationToken = default);
    IQueryable<Domain.Entities.Lawyer> Query();
}