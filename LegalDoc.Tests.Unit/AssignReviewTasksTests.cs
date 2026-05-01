using LegalDoc.Application.Abstractions;
using LegalDoc.Application.ReviewTask.Commands;
using LegalDoc.Domain.Entities;
using LegalDoc.Domain.Enums;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class AssignReviewTaskTests
{
    private readonly Mock<IDocumentsRepository> _docRepoMock;
    private readonly Mock<ILawyerRepository> _lawyerRepoMock;
    private readonly Mock<IReviewTaskRepository> _taskRepoMock;
    private readonly AssignReviewTaskCommandHandler _handler;

    public AssignReviewTaskTests()
    {
        _docRepoMock = new Mock<IDocumentsRepository>();
        _lawyerRepoMock = new Mock<ILawyerRepository>();
        _taskRepoMock = new Mock<IReviewTaskRepository>();

        _handler = new AssignReviewTaskCommandHandler(
            _docRepoMock.Object, 
            _lawyerRepoMock.Object, 
            _taskRepoMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenLawyerIsNotActive()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var lawyerId = Guid.NewGuid();
        var command = new AssignReviewTaskCommand(docId, lawyerId, "Descriere");

        var document = LegalDocument.Create("Titlu", "file.pdf", "/path", "Content", Guid.NewGuid());
        
        // --- IMPORTANT: Fortam statusul corect ---
        // Daca ai o metoda in Domain care face asta (ex: document.MarkAsAwaitingReview()), foloseste-o.
        // Daca nu, va trebui sa simulam ca documentul este gata. 
        // Presupunem aici ca statusul trebuie sa fie AwaitingReview.
        typeof(LegalDocument).GetProperty(nameof(LegalDocument.Status))?
            .SetValue(document, DocumentStatus.AwaitingReview);

        var lawyer = Lawyer.Create("Avocat Test", "123", "test@mail.com");
        lawyer.UpdateLawyerActivity(false); // Il facem inactiv

        _docRepoMock.Setup(x => x.FindAsync(docId, It.IsAny<CancellationToken>())).ReturnsAsync(document);
        _lawyerRepoMock.Setup(x => x.FindAsync(lawyerId, It.IsAny<CancellationToken>())).ReturnsAsync(lawyer);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Acum ar trebui sa treaca de verificarea documentului si sa ajunga la cea a avocatului
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Lawyer is not active.");
    }

    [Fact]
    public async Task Handle_Should_AssignTask_Successfully_WhenAllConditionsAreMet()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var lawyerId = Guid.NewGuid();
        var command = new AssignReviewTaskCommand(docId, lawyerId, "Review urgent");

        var document = LegalDocument.Create("Titlu", "file.pdf", "/path", "Content", Guid.NewGuid());
        
        // Fortam statusul documentului sa fie cel asteptat de Handler
        typeof(LegalDocument).GetProperty(nameof(LegalDocument.Status))?
            .SetValue(document, DocumentStatus.AwaitingReview);

        var lawyer = Lawyer.Create("Avocat Activ", "123", "active@mail.com");

        _docRepoMock.Setup(x => x.FindAsync(docId, It.IsAny<CancellationToken>())).ReturnsAsync(document);
        _lawyerRepoMock.Setup(x => x.FindAsync(lawyerId, It.IsAny<CancellationToken>())).ReturnsAsync(lawyer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        document.Status.Should().Be(DocumentStatus.InReview);
        _taskRepoMock.Verify(x => x.AddAsync(It.IsAny<ReviewTask>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}