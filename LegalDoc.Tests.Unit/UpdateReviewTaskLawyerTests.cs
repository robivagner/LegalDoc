using LegalDoc.Application.Abstractions;
using LegalDoc.Application.ReviewTask.Commands;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class UpdateReviewTaskLawyerTests
{
    private readonly Mock<IReviewTaskRepository> _taskRepoMock;
    private readonly Mock<ILawyerRepository> _lawyerRepoMock;
    private readonly UpdateReviewTaskLawyerCommandHandler _handler;

    public UpdateReviewTaskLawyerTests()
    {
        // Initializam Mock-urile
        _taskRepoMock = new Mock<IReviewTaskRepository>();
        _lawyerRepoMock = new Mock<ILawyerRepository>();

        // Initializam Handler-ul folosind obiectele simulate
        _handler = new UpdateReviewTaskLawyerCommandHandler(
            _taskRepoMock.Object, 
            _lawyerRepoMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenOldLawyerIsStillActive()
    {
        // Acest test il aveai deja, dar acum va folosi campurile de mai sus
        var oldLawyerId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var oldLawyer = Lawyer.Create("Vechiul Avocat", "123", "v@mail.com");
        var reviewTask = ReviewTask.Create(Guid.NewGuid(), oldLawyerId, "Review");

        _taskRepoMock.Setup(x => x.FindAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(reviewTask);
        _lawyerRepoMock.Setup(x => x.FindAsync(oldLawyerId, It.IsAny<CancellationToken>())).ReturnsAsync(oldLawyer);

        var command = new UpdateReviewTaskLawyerCommand(taskId, Guid.NewGuid());

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Active lawyer cannot be replaced.");
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNewLawyerIsInactive()
    {
        // Acesta este testul nou care dadea erori
        var taskId = Guid.NewGuid();
        var oldLawyerId = Guid.NewGuid();
        var newLawyerId = Guid.NewGuid();
        
        var oldLawyer = Lawyer.Create("Vechi", "1", "v@t.com");
        oldLawyer.UpdateLawyerActivity(false); // Trebuie sa fie inactiv ca sa trecem de prima verificare
        
        var newLawyer = Lawyer.Create("Nou", "2", "n@t.com");
        newLawyer.UpdateLawyerActivity(false); // Noul e inactiv -> eroare asteptata

        var task = ReviewTask.Create(Guid.NewGuid(), oldLawyerId, "Desc");

        _taskRepoMock.Setup(x => x.FindAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _lawyerRepoMock.Setup(x => x.FindAsync(oldLawyerId, It.IsAny<CancellationToken>())).ReturnsAsync(oldLawyer);
        _lawyerRepoMock.Setup(x => x.FindAsync(newLawyerId, It.IsAny<CancellationToken>())).ReturnsAsync(newLawyer);

        var command = new UpdateReviewTaskLawyerCommand(taskId, newLawyerId);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("New lawyer is not active.");
    }
}