using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Registry.Commands;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class CreateRegistryTests
{
    [Fact]
    public async Task Handle_Should_ReturnGuid_WhenRegistryIsCreated()
    {
        // Arrange
        var repoMock = new Mock<IRegistryRepository>();
        var handler = new CreateRegistryCommandHandler(repoMock.Object);
        var command = new CreateRegistryCommand("Arhiva Nord", "Suceava", 500);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        repoMock.Verify(x => x.AddAsync(It.IsAny<Registry>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}