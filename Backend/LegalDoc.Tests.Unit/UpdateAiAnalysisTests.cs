using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Document.Commands;
using LegalDoc.Application.Document.Queries;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class UpdateAiAnalysisTests
{
    [Fact]
    public async Task Handle_Should_UpdateDocumentFields_UsingAiService_WhenDocumentExists()
    {
        // Arrange
        var repoMock = new Mock<IDocumentsRepository>();
        var aiServiceMock = new Mock<IAiService>(); // 1. Creăm Mock pentru noul serviciu
        
        var docId = Guid.NewGuid();
        var document = LegalDocument.Create("Titlu", "file.pdf", "/path", "Conținut document de test", Guid.NewGuid());
        
        // Mock pentru Repository
        repoMock.Setup(x => x.FindAsync(docId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        // 2. Configurăm Mock-ul de AI să returneze un răspuns simulat
        var aiResponse = new AiAnalysisResponse(
            "Summary din AI", 
            "Clauze din AI", 
            "Riscuri din AI"
        );

        aiServiceMock.Setup(x => x.AnalyzeDocumentAsync(document.Content))
            .ReturnsAsync(aiResponse);

        var command = new UpdateDocumentAiAnalysisCommand(docId);
        
        // 3. Trimitem ambele Mock-uri în constructor
        var handler = new UpdateDocumentAiAnalysisCommandHandler(repoMock.Object, aiServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        // Verificăm dacă documentul a fost actualizat cu ce a returnat AI-ul, nu comanda
        document.Summary.Should().Be("Summary din AI");
        document.Clauses.Should().Be("Clauze din AI");
        document.Risks.Should().Be("Riscuri din AI");
        
        repoMock.Verify(x => x.UpdateAsync(document, It.IsAny<CancellationToken>()), Times.Once);
        aiServiceMock.Verify(x => x.AnalyzeDocumentAsync(It.IsAny<string>()), Times.Once);
    }
}