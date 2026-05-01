using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Lawyer.Commands;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class UpdateLawyerActivityTests
{
    private readonly Mock<ILawyerRepository> _repoMock;
    private readonly UpdateLawyerActivityCommandHandler _handler;

    public UpdateLawyerActivityTests()
    {
        _repoMock = new Mock<ILawyerRepository>();
        _handler = new UpdateLawyerActivityCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateStatus_WhenLawyerExists()
    {
        // Arrange
        var lawyerId = Guid.NewGuid();
        var lawyer = Lawyer.Create("Avocat", "123", "a@test.com");
        _repoMock.Setup(x => x.FindAsync(lawyerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lawyer);

        var command = new UpdateLawyerActivityCommand(lawyerId, false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        lawyer.IsActive.Should().BeFalse();
        _repoMock.Verify(x => x.UpdateAsync(lawyer, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenLawyerNotFound()
    {
        // Arrange
        _repoMock.Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lawyer)null!);

        var command = new UpdateLawyerActivityCommand(Guid.NewGuid(), true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Lawyer not found.");
    }
}