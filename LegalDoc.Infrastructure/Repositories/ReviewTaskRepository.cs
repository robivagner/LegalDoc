using LegalDoc.Application.Abstractions;
using LegalDoc.Domain.Entities;
using LegalDoc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalDoc.Infrastructure.Repositories;

public class ReviewTaskRepository(AppDbContext dbContext) : IReviewTaskRepository
{
    public async Task AddAsync(ReviewTask task, CancellationToken cancellationToken = default)
    {
        await dbContext.ReviewTasks.AddAsync(task, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ReviewTask?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.ReviewTasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(ReviewTask task, CancellationToken cancellationToken = default)
    {
        dbContext.ReviewTasks.Update(task);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<ReviewTask> Query()
    {
        return dbContext.ReviewTasks.AsQueryable();
    }
}