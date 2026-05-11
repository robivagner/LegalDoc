using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Lawyer.Commands;
using LegalDoc.Domain.Entities;
using Moq;

namespace LegalDoc.Tests.Unit.LawyersTests;

public class CreateLawyerTests
{
    [Fact]
    public async Task Handle_Should_ReturnValidGuid_WhenLawyerIsCreated()
    {
        // Arrange
        var repoMock = new Mock<ILawyerRepository>();
        var handler = new CreateLawyerCommandHandler(repoMock.Object);
        var command = new CreateLawyerCommand(Guid.NewGuid(), "Ion Popescu", "RO12345", "ion@popescu.ro");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty(); // Verificam ca am primit un Guid
        repoMock.Verify(x => x.AddAsync(It.IsAny<Lawyer>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public void Create_EmptyId_ThrowsArgumentException()
    {
        var act = () => Lawyer.Create(Guid.Empty, "Nume", "BAR123", "test@email.com");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Lawyer ID cannot be empty.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_InvalidName_ThrowsArgumentException(string? invalidName)
    {
        var act = () => Lawyer.Create(Guid.NewGuid(), invalidName!, "BAR123", "test@email.com");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Lawyer name cannot be null or empty.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_InvalidBarNumber_ThrowsArgumentException(string? invalidBar)
    {
        var act = () => Lawyer.Create(Guid.NewGuid(), "Nume", invalidBar!, "test@email.com");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Lawyer bar number cannot be null or empty.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_InvalidEmail_ThrowsArgumentException(string? invalidEmail)
    {
        var act = () => Lawyer.Create(Guid.NewGuid(), "Nume", "BAR123", invalidEmail!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Lawyer email cannot be null or empty.*");
    }

    [Fact]
    public void Create_ValidData_ReturnsLawyer()
    {
        var id = Guid.NewGuid();
        var result = Lawyer.Create(id, "Robert", "BAR123", "robert@test.com");

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.IsActive.Should().BeTrue();
    }
}