using LegalDoc.Application.Document.Queries;
using LegalDoc.Infrastructure.Repositories;
using LegalDoc.Domain.Entities;
using LegalDoc.Domain.Enums;
using FluentAssertions;

namespace LegalDoc.Tests.Integration;

public class DocumentQueryIntegrationTests
{
    private readonly TestDatabaseFixture _fixture = new();

    [Fact]
    public async Task GetDocumentsQuery_ShouldReturnOnlySpecificStatus()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var regId = Guid.NewGuid();
        
        var doc1 = LegalDocument.Create("Doc Complet", "1.pdf", "/p", "Content", regId);
        var doc2 = LegalDocument.Create("Doc In Lucru", "2.pdf", "/p", "Content", regId);

        // Fortam statusuri diferite
        typeof(LegalDocument).GetProperty(nameof(LegalDocument.Status))?.SetValue(doc1, DocumentStatus.Completed);
        typeof(LegalDocument).GetProperty(nameof(LegalDocument.Status))?.SetValue(doc2, DocumentStatus.InReview);

        context.LegalDocuments.AddRange(doc1, doc2);
        await context.SaveChangesAsync();

        var repo = new DocumentsRepository(context);
        var handler = new GetDocumentsQueryHandler(repo);
        
        // Cautam doar cele Completed
        var query = new GetDocumentsQuery(DocumentStatus.Completed);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Doc Complet");
    }
}