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
    
    [Fact]
    public async Task LawyerRepository_FullCycle_ShouldWork()
    {
        // 1. Setup - Folosim baza de date de test (InMemory sau Postgres de test)
        var db = GetDbContext(); // Metoda ta care creează contextul
        var repo = new LawyerRepository(db);
        var lawyerId = Guid.NewGuid();
        var lawyer = Lawyer.Create(lawyerId, "Mirel Avocat", "RO123", "mirel@test.com");

        // 2. Test ADD (Acoperă AddAsync)
        await repo.AddAsync(lawyer, default);
        await db.SaveChangesAsync();

        // 3. Test FIND (Acoperă FindAsync)
        var found = await repo.FindAsync(lawyerId, default);
        found.Should().NotBeNull();
        found!.Name.Should().Be("Mirel Avocat");

        // 4. Test UPDATE (Acoperă UpdateAsync - linia care acum e pe 0%)
        found.UpdateLawyerActivity(false); // Presupunem că ai metoda asta în Domain
        await repo.UpdateAsync(found, default);
        await db.SaveChangesAsync();

        var updated = await repo.FindAsync(lawyerId, default);
        updated!.IsActive.Should().BeFalse();

        // 5. Test QUERY (Acoperă metoda Query() - IQueryable)
        var list = repo.Query().Where(x => x.BarNumber == "RO123").ToList();
        list.Should().ContainSingle();
    }
    
    [Fact]
    public async Task RegistryRepository_FullCycle_ShouldWork()
    {
        // Arrange
        var db = GetDbContext();
        var repo = new RegistryRepository(db);
        var registry = Registry.Create("Registru Central", "Bucuresti", 100);

        // 1. Test AddAsync
        await repo.AddAsync(registry, default);
        // SaveChanges este deja inclus în AddAsync conform codului tău, 
        // dar db.SaveChangesAsync() aici nu strică pentru siguranță.

        // 2. Test FindAsync
        var found = await repo.FindAsync(registry.Id, default);
        found.Should().NotBeNull();
        found!.Name.Should().Be("Registru Central");

        // 3. Test Query (IQueryable)
        var exists = repo.Query().Any(x => x.Location == "Bucuresti");
        exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task ReviewTaskRepository_FullCycle_ShouldWork()
    {
        // Arrange
        var db = GetDbContext();
        var repo = new ReviewTaskRepository(db);
        var documentId = Guid.NewGuid();
        var lawyerId = Guid.NewGuid();
        var task = ReviewTask.Create(documentId, lawyerId, "Revizuire contract vanzare-cumparare");

        // 1. Test AddAsync
        await repo.AddAsync(task, default);

        // 2. Test FindAsync
        var found = await repo.FindAsync(task.Id, default);
        found.Should().NotBeNull();
        found!.Description.Should().Be("Revizuire contract vanzare-cumparare");

        // 3. Test UpdateAsync (Aceasta este linia care lipsește de obicei)
        task.UpdateStatus(ReviewTaskStatus.Completed);
        await repo.UpdateAsync(task, default);
        await db.SaveChangesAsync();

        var updated = await repo.FindAsync(task.Id, default);
        updated!.Status.Should().Be(ReviewTaskStatus.Completed);

        // 4. Test Query
        var hasTask = repo.Query().Any(x => x.DocumentId == documentId);
        hasTask.Should().BeTrue();
    }
}