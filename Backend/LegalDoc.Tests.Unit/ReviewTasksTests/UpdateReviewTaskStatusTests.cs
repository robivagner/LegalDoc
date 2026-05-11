using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.ReviewTask.Commands;
using LegalDoc.Domain.Entities;
using LegalDoc.Domain.Enums;
using Moq;

namespace LegalDoc.Tests.Unit.ReviewTasksTests;

public class UpdateReviewTaskStatusTests
{
    private readonly Mock<IReviewTaskRepository> _taskRepoMock = new();
    private readonly Mock<IDocumentsRepository> _docRepoMock = new();
    private readonly Mock<ILawyerRepository> _lawyerRepoMock = new();

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _taskRepoMock.Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReviewTask)null!);
        var handler = new UpdateReviewTaskStatusCommandHandler(_taskRepoMock.Object, _docRepoMock.Object, _lawyerRepoMock.Object);

        // Act
        var act = () => handler.Handle(new UpdateReviewTaskStatusCommand(Guid.NewGuid(), ReviewTaskStatus.Completed), default);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Review task not found.");
    }

    [Fact]
    public async Task Handle_StatusCompleted_UpdatesDocumentToCompleted()
    {
        // Arrange
        var doc = LegalDocument.Create("T", "C", "aici","content", Guid.NewGuid());
        var lawyer = Lawyer.Create(Guid.NewGuid(),"A", "e", "s");
        var task = ReviewTask.Create(doc.Id, lawyer.Id, "D");

        _taskRepoMock.Setup(x => x.FindAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _lawyerRepoMock.Setup(x => x.FindAsync(lawyer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lawyer);
        _docRepoMock.Setup(x => x.FindAsync(doc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(doc);

        var handler = new UpdateReviewTaskStatusCommandHandler(_taskRepoMock.Object, _docRepoMock.Object, _lawyerRepoMock.Object);

        // Act
        await handler.Handle(new UpdateReviewTaskStatusCommand(task.Id, ReviewTaskStatus.Completed), default);

        // Assert
        task.Status.Should().Be(ReviewTaskStatus.Completed);
        doc.Status.Should().Be(DocumentStatus.Completed);
        _docRepoMock.Verify(x => x.UpdateAsync(doc, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_StatusRejected_UpdatesDocumentToRejected()
    {
        // Arrange
        var doc = LegalDocument.Create("T", "C", "aici","content", Guid.NewGuid());
        var lawyer = Lawyer.Create(Guid.NewGuid(), "A", "e", "s");
        var task = ReviewTask.Create(doc.Id, lawyer.Id, "D");

        _taskRepoMock.Setup(x => x.FindAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _lawyerRepoMock.Setup(x => x.FindAsync(lawyer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lawyer);
        _docRepoMock.Setup(x => x.FindAsync(doc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(doc);

        var handler = new UpdateReviewTaskStatusCommandHandler(_taskRepoMock.Object, _docRepoMock.Object, _lawyerRepoMock.Object);

        // Act
        await handler.Handle(new UpdateReviewTaskStatusCommand(task.Id, ReviewTaskStatus.Rejected), default);

        // Assert
        doc.Status.Should().Be(DocumentStatus.Rejected);
    }
}