using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Lawyer.Queries;
using LegalDoc.Domain.Entities;
using Moq;

namespace LegalDoc.Tests.Unit.LawyersTests;

public class GetLawyersTests
{
    private readonly Mock<ILawyerRepository> _repositoryMock = new();
    
    [Fact]
    public async Task Handle_Should_ReturnLawyerList()
    {
        // Arrange
        var data = new List<Lawyer> 
        { 
            Lawyer.Create(Guid.NewGuid(), "Andrei Ionescu", "BAR777", "andrei@law.ro") 
        }.AsQueryable();

        _repositoryMock.Setup(x => x.Query()).Returns(data);
        var handler = new GetLawyersQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLawyersQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        result.First().Name.Should().Be("Andrei Ionescu");
    }
    
    [Fact]
    public async Task Handle_LawyerExists_ReturnsLawyer()
    {
        // Arrange
        var lawyerId = Guid.NewGuid();
        var lawyer = Lawyer.Create(lawyerId, "Robert Vagner", "BAR12345", "robert@test.com");
        
        _repositoryMock.Setup(x => x.FindAsync(lawyerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lawyer);

        var handler = new GetLawyerQueryHandler(_repositoryMock.Object);
        var query = new GetLawyerQuery(lawyerId);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(lawyerId);
        result.Name.Should().Be("Robert Vagner");
    }

    [Fact]
    public async Task Handle_LawyerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var lawyerId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.FindAsync(lawyerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lawyer?)null);

        var handler = new GetLawyerQueryHandler(_repositoryMock.Object);
        var query = new GetLawyerQuery(lawyerId);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.Should().BeNull();
    }
}