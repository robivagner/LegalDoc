using Microsoft.EntityFrameworkCore;
using LegalDoc.Infrastructure.Persistence;
using LegalDoc.Infrastructure.Repositories;
using LegalDoc.Domain.Entities;
using FluentAssertions;
using LegalDoc.Domain.Enums;

namespace LegalDoc.Tests.Integration;

public class RepositoryTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task DocumentsRepository_AddAndFind_ShouldWork()
    {
        var db = GetDbContext();
        var repo = new DocumentsRepository(db);
        var doc = LegalDocument.Create("Titlu", "f.pdf", "/p", "con", Guid.NewGuid());

        await repo.AddAsync(doc, default);
        await db.SaveChangesAsync();

        var result = await repo.FindAsync(doc.Id, CancellationToken.None);
        result.Should().NotBeNull();
        result!.Title.Should().Be("Titlu");
    }
    
    [Fact]
    public async Task LawyerRepository_AddAndFind_ShouldWork()
    {
        var db = GetDbContext();
        var repo = new LawyerRepository(db);
        var lawyer = Lawyer.Create(Guid.NewGuid(), "Avocat", "BAR1", "a@a.com");

        await repo.AddAsync(lawyer, CancellationToken.None);
        await db.SaveChangesAsync();

        var result = await repo.FindAsync(lawyer.Id, CancellationToken.None);
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DocumentsRepository_Update_ShouldWork()
    {
        var db = GetDbContext();
        var repo = new DocumentsRepository(db);
        var doc = LegalDocument.Create("Titlu", "f.pdf", "/p", "con", Guid.NewGuid());
        await repo.AddAsync(doc, default);
        await db.SaveChangesAsync();

        doc.MarkAsCompleted(); // Schimbăm ceva
        await repo.UpdateAsync(doc, default);
        await db.SaveChangesAsync();

        var updated = await repo.FindAsync(doc.Id, default);
        updated!.Status.Should().Be(DocumentStatus.Completed);
    }
    
    [Fact]
    public async Task Repositories_Update_ShouldWork()
    {
        var db = GetDbContext();
        var repo = new DocumentsRepository(db);
        var doc = LegalDocument.Create("Titlu Original", "file.pdf", "/path", "content", Guid.NewGuid());
        await repo.AddAsync(doc, default);
        await db.SaveChangesAsync();

        // Act: Update
        var existing = await repo.FindAsync(doc.Id, default);
        // Presupunem că ai o metodă sau proprietate de modificat, ex: Status
        existing!.MarkAsCompleted(); 
        await repo.UpdateAsync(existing, default);
        await db.SaveChangesAsync();

        // Assert
        var updated = await repo.FindAsync(doc.Id, default);
        updated!.Status.Should().Be(DocumentStatus.Completed);
    }
}