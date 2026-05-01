using LegalDoc.Infrastructure.Repositories;
using LegalDoc.Domain.Entities;
using LegalDoc.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace LegalDoc.Tests.Integration;

public class ReviewTaskIntegrationTests
{
    private readonly TestDatabaseFixture _fixture = new();

    [Fact]
    public async Task CreateReviewTask_ShouldLinkEverythingCorrectly()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        
        // 1. Cream datele necesare (Lawyer si Document)
        var lawyer = Lawyer.Create("Avocat Test", "BAR123", "avocat@test.ro");
        var document = LegalDocument.Create("Contract Test", "test.pdf", "/path", "Content", Guid.NewGuid());
        
        // Setam statusul documentului sa fie gata de review (folosind Reflection daca e nevoie)
        typeof(LegalDocument).GetProperty(nameof(LegalDocument.Status))?
            .SetValue(document, DocumentStatus.AwaitingReview);

        context.Lawyers.Add(lawyer);
        context.LegalDocuments.Add(document); // Folosim numele corect gasit de tine
        await context.SaveChangesAsync();

        var taskRepo = new ReviewTaskRepository(context);
        
        // 2. Cream task-ul de review
        var task = ReviewTask.Create(document.Id, lawyer.Id, "Verificare clauze");

        // Act
        await taskRepo.AddAsync(task);
        await context.SaveChangesAsync();

        // Assert
        var savedTask = await context.ReviewTasks
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        savedTask.Should().NotBeNull();
        savedTask!.Description.Should().Be("Verificare clauze");
        savedTask.LawyerId.Should().Be(lawyer.Id);
        savedTask.DocumentId.Should().Be(document.Id);
    }
}