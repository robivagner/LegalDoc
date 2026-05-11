using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Registry.Commands;
using LegalDoc.Domain.Entities;
using Moq;

namespace LegalDoc.Tests.Unit.RegistriesTests;

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
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_InvalidName_ThrowsArgumentException(string? invalidName)
    {
        var act = () => Registry.Create(invalidName!, "Bucuresti", 100);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Department name cannot be empty.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_InvalidLocation_ThrowsArgumentException(string? invalidLocation)
    {
        var act = () => Registry.Create("Arhiva", invalidLocation!, 100);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Department location cannot be empty.*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_InvalidCapacity_ThrowsArgumentException(int invalidCapacity)
    {
        var act = () => Registry.Create("Arhiva", "Bucuresti", invalidCapacity);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Capacity needs to be greater than 0*");
    }

    [Fact]
    public void Create_ValidData_ReturnsRegistry()
    {
        var result = Registry.Create("Arhiva Centrala", "Iasi", 500);

        result.Should().NotBeNull();
        result.Capacity.Should().Be(500);
        result.Availability.Should().Be(500);
        result.Name.Should().Be("Arhiva Centrala");
    }
}