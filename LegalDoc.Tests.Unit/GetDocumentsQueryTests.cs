using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Document.Queries;
using LegalDoc.Domain.Entities;
using LegalDoc.Domain.Enums;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class GetDocumentsQueryTests
{
    [Fact]
    public async Task Handle_Should_ReturnOnlyFilteredDocuments_WhenStatusIsProvided()
    {
        // Arrange
        var docRepoMock = new Mock<IDocumentsRepository>();
        
        // Cream o lista de documente "fake"
        var registryId = Guid.NewGuid();
        var data = new List<LegalDocument>
        {
            LegalDocument.Create("Doc 1", "file1.pdf", "/path", "Content", registryId),
            LegalDocument.Create("Doc 2", "file2.pdf", "/path", "Content", registryId)
        }.AsQueryable();

        // Fortam statusul primului document sa fie diferit folosind Reflection
        typeof(LegalDocument).GetProperty(nameof(LegalDocument.Status))?
            .SetValue(data.First(), DocumentStatus.Completed);

        docRepoMock.Setup(x => x.Query()).Returns(data);

        var handler = new GetDocumentsQueryHandler(docRepoMock.Object);
        var query = new GetDocumentsQuery(DocumentStatus.Completed);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Doc 1");
    }
}