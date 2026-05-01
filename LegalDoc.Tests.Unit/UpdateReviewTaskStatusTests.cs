using LegalDoc.Application.Abstractions;
using LegalDoc.Application.ReviewTask.Commands;
using LegalDoc.Domain.Entities;
using LegalDoc.Domain.Enums;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class UpdateReviewTaskStatusTests
{
    private readonly Mock<IReviewTaskRepository> _taskRepoMock;
    private readonly Mock<IDocumentsRepository> _docRepoMock;
    private readonly Mock<ILawyerRepository> _lawyerRepoMock;
    private readonly UpdateReviewTaskStatusCommandHandler _handler;

    public UpdateReviewTaskStatusTests()
    {
        _taskRepoMock = new Mock<IReviewTaskRepository>();
        _docRepoMock = new Mock<IDocumentsRepository>();
        _lawyerRepoMock = new Mock<ILawyerRepository>();

        _handler = new UpdateReviewTaskStatusCommandHandler(
            _taskRepoMock.Object,
            _docRepoMock.Object,
            _lawyerRepoMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateDocumentToCompleted_WhenTaskIsCompleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var docId = Guid.NewGuid();
        var lawyerId = Guid.NewGuid();

        // Cream obiectele de Domain
        var document = LegalDocument.Create("Titlu", "file.pdf", "/path", "Content", Guid.NewGuid());
        var lawyer = Lawyer.Create("Avocat", "123", "a@a.com");
        var reviewTask = ReviewTask.Create(docId, lawyerId, "Review initial");

        // Simulam repository-urile sa returneze obiectele noastre
        _taskRepoMock.Setup(x => x.FindAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(reviewTask);
        _lawyerRepoMock.Setup(x => x.FindAsync(lawyerId, It.IsAny<CancellationToken>())).ReturnsAsync(lawyer);
        _docRepoMock.Setup(x => x.FindAsync(docId, It.IsAny<CancellationToken>())).ReturnsAsync(document);

        var command = new UpdateReviewTaskStatusCommand(taskId, ReviewTaskStatus.Completed);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // 1. Verificam ca statusul task-ului s-a schimbat
        reviewTask.Status.Should().Be(ReviewTaskStatus.Completed);
        
        // 2. Verificam efectul de domino asupra documentului
        document.Status.Should().Be(DocumentStatus.Completed);

        // 3. Verificam ca ambele entitati au fost salvate in baza de date
        _taskRepoMock.Verify(x => x.UpdateAsync(reviewTask, It.IsAny<CancellationToken>()), Times.Once);
        _docRepoMock.Verify(x => x.UpdateAsync(document, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenLawyerIsInactive()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var lawyerId = Guid.NewGuid();
        
        var lawyer = Lawyer.Create("Avocat", "123", "a@a.com");
        lawyer.UpdateLawyerActivity(false); // Avocatul devine inactiv intre timp
        
        var reviewTask = ReviewTask.Create(Guid.NewGuid(), lawyerId, "Review");

        _taskRepoMock.Setup(x => x.FindAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(reviewTask);
        _lawyerRepoMock.Setup(x => x.FindAsync(lawyerId, It.IsAny<CancellationToken>())).ReturnsAsync(lawyer);

        var command = new UpdateReviewTaskStatusCommand(taskId, ReviewTaskStatus.Completed);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Lawyer is not active.");
    }
}