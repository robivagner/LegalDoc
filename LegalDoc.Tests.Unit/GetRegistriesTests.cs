using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Registry.Queries;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class GetRegistriesTests
{
    [Fact]
    public async Task Handle_Should_ReturnAllRegistries()
    {
        // Arrange
        var repoMock = new Mock<IRegistryRepository>();
        var data = new List<Registry> 
        { 
            Registry.Create("Depozit 1", "Cluj", 100) 
        }.AsQueryable();

        repoMock.Setup(x => x.Query()).Returns(data);
        var handler = new GetRegistriesQueryHandler(repoMock.Object);

        // Act
        var result = await handler.Handle(new GetRegistriesQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Depozit 1");
        result.First().Availability.Should().Be(100);
    }
}