using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Lawyer.Commands;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class CreateLawyerTests
{
    [Fact]
    public async Task Handle_Should_ReturnValidGuid_WhenLawyerIsCreated()
    {
        // Arrange
        var repoMock = new Mock<ILawyerRepository>();
        var handler = new CreateLawyerCommandHandler(repoMock.Object);
        var command = new CreateLawyerCommand("Ion Popescu", "RO12345", "ion@popescu.ro");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty(); // Verificam ca am primit un Guid
        repoMock.Verify(x => x.AddAsync(It.IsAny<Lawyer>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}