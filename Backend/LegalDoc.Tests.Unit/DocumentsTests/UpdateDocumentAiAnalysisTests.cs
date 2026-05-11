using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Document.Commands;
using LegalDoc.Application.Document.Queries;
using LegalDoc.Domain.Entities;
using Moq;

namespace LegalDoc.Tests.Unit.DocumentsTests;

public class UpdateDocumentAiAnalysisTests
{
    private readonly Mock<IDocumentsRepository> _repositoryMock = new();
    private readonly Mock<IAiService> _aiServiceMock = new();

    // Helper pentru a crea un document valid cu 5 parametri
    private LegalDocument CreateDummyDocument()
    {
        return LegalDocument.Create(
            "Titlu Document", 
            "contract.pdf", 
            "/storage/path", 
            "Conținut document de test", 
            Guid.NewGuid()
        );
    }

    [Fact]
    public async Task Handle_Should_UpdateDocumentFields_UsingAiService_WhenDocumentExists()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var document = CreateDummyDocument();
        
        _repositoryMock.Setup(x => x.FindAsync(docId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        // Folosim AiAnalysisResponse (care pare a fi tipul corect în proiectul tău)
        var aiResponse = new AiAnalysisResponse(
            "Summary din AI", 
            "Clauze din AI", 
            "Riscuri din AI"
        );

        _aiServiceMock.Setup(x => x.AnalyzeDocumentAsync(document.Content))
            .ReturnsAsync(aiResponse);

        var command = new UpdateDocumentAiAnalysisCommand(docId);
        var handler = new UpdateDocumentAiAnalysisCommandHandler(_repositoryMock.Object, _aiServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        document.Summary.Should().Be("Summary din AI");
        document.Clauses.Should().Be("Clauze din AI");
        document.Risks.Should().Be("Riscuri din AI");
        
        _repositoryMock.Verify(x => x.UpdateAsync(document, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DocumentNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var docId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.FindAsync(docId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LegalDocument?)null);
            
        var handler = new UpdateDocumentAiAnalysisCommandHandler(_repositoryMock.Object, _aiServiceMock.Object);

        // Act
        var act = () => handler.Handle(new UpdateDocumentAiAnalysisCommand(docId), default);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Document not found.");
    }

    [Fact]
    public async Task Handle_AiAnalysisFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var document = CreateDummyDocument();
        _repositoryMock.Setup(x => x.FindAsync(document.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);
            
        // Simulează eșecul serviciului AI
        _aiServiceMock.Setup(x => x.AnalyzeDocumentAsync(document.Content))
            .ReturnsAsync((AiAnalysisResponse?)null);

        var handler = new UpdateDocumentAiAnalysisCommandHandler(_repositoryMock.Object, _aiServiceMock.Object);

        // Act
        var act = () => handler.Handle(new UpdateDocumentAiAnalysisCommand(document.Id), default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("AI analysis error.");
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsUpdateOnRepository()
    {
        // Arrange
        var document = CreateDummyDocument();
        var aiResponse = new AiAnalysisResponse("Sum", "Clau", "Risk");
        
        _repositoryMock.Setup(x => x.FindAsync(document.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);
        _aiServiceMock.Setup(x => x.AnalyzeDocumentAsync(document.Content))
            .ReturnsAsync(aiResponse);
            
        var handler = new UpdateDocumentAiAnalysisCommandHandler(_repositoryMock.Object, _aiServiceMock.Object);

        // Act
        await handler.Handle(new UpdateDocumentAiAnalysisCommand(document.Id), default);

        // Assert
        _repositoryMock.Verify(x => x.UpdateAsync(document, It.IsAny<CancellationToken>()), Times.Once);
        document.Summary.Should().Be("Sum");
    }
    
    [Fact]
    public void LegalDocument_UpdateAiAnalysis_WrongStatus_ThrowsException()
    {
        var doc = LegalDocument.Create("T", "F", "P", "C", Guid.NewGuid());
        doc.MarkAsCompleted(); // Statusul devine Completed
    
        // Act
        var act = () => doc.UpdateAiAnalysis("S", "C", "R");
    
        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}