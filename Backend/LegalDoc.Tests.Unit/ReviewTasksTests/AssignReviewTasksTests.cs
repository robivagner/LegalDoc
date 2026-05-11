using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.ReviewTask.Commands;
using LegalDoc.Domain.Entities;
using LegalDoc.Domain.Enums;
using Moq;

namespace LegalDoc.Tests.Unit.ReviewTasksTests;

public class AssignReviewTaskTests
{
    private readonly Mock<IDocumentsRepository> _docRepoMock = new();
    private readonly Mock<ILawyerRepository> _lawyerRepoMock = new();
    private readonly Mock<IReviewTaskRepository> _taskRepoMock = new();

    // Helper pentru a crea un document gata de review
    private LegalDocument CreateDocumentReadyForReview()
    {
        var doc = LegalDocument.Create("Titlu", "file.pdf", "/path", "continut", Guid.NewGuid());
        
        // Simulăm trecerea prin AI pentru a schimba statusul din Uploaded în AwaitingReview
        doc.UpdateAiAnalysis("Sumar", "Clauze", "Riscuri");
        
        return doc;
    }

    [Fact]
    public async Task Handle_DocumentInvalidStatus_ThrowsInvalidOperationException()
    {
        // Arrange: Cream un document care e doar Uploaded (nu apelăm UpdateAiAnalysis)
        var doc = LegalDocument.Create("T", "F", "P", "C", Guid.NewGuid());
        
        _docRepoMock.Setup(x => x.FindAsync(doc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(doc);
        var handler = new AssignReviewTaskCommandHandler(_docRepoMock.Object, _lawyerRepoMock.Object, _taskRepoMock.Object);

        // Act
        var act = () => handler.Handle(new AssignReviewTaskCommand(doc.Id, Guid.NewGuid(), "Desc"), default);

        // Assert
        // Va pica pentru că statusul este Uploaded, nu AwaitingReview
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Document is not awaiting review.");
    }

    [Fact]
    public async Task Handle_LawyerInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var doc = CreateDocumentReadyForReview(); // Status: AwaitingReview
        var lawyer = Lawyer.Create(Guid.NewGuid(), "Nume", "email@test.com", "Specializare");
        lawyer.UpdateLawyerActivity(false); // Inactiv

        _docRepoMock.Setup(x => x.FindAsync(doc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(doc);
        _lawyerRepoMock.Setup(x => x.FindAsync(lawyer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lawyer);
        
        var handler = new AssignReviewTaskCommandHandler(_docRepoMock.Object, _lawyerRepoMock.Object, _taskRepoMock.Object);

        // Act
        var act = () => handler.Handle(new AssignReviewTaskCommand(doc.Id, lawyer.Id, "Desc"), default);

        // Assert
        // Acum trece de validarea documentului și ajunge la validarea avocatului
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Lawyer is not active.");
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesTaskAndReturnsGuid()
    {
        // Arrange
        var doc = CreateDocumentReadyForReview(); // Status: AwaitingReview
        var lawyer = Lawyer.Create(Guid.NewGuid(), "Nume", "email@test.com", "Specializare");

        _docRepoMock.Setup(x => x.FindAsync(doc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(doc);
        _lawyerRepoMock.Setup(x => x.FindAsync(lawyer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lawyer);
        
        var handler = new AssignReviewTaskCommandHandler(_docRepoMock.Object, _lawyerRepoMock.Object, _taskRepoMock.Object);

        // Act
        var resultId = await handler.Handle(new AssignReviewTaskCommand(doc.Id, lawyer.Id, "Desc"), default);

        // Assert
        resultId.Should().NotBeEmpty();
        doc.Status.Should().Be(DocumentStatus.InReview);
        _taskRepoMock.Verify(x => x.AddAsync(It.IsAny<ReviewTask>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}