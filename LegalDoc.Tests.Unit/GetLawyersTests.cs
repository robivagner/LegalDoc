using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Lawyer.Queries;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class GetLawyersTests
{
    [Fact]
    public async Task Handle_Should_ReturnLawyerList()
    {
        // Arrange
        var repoMock = new Mock<ILawyerRepository>();
        var data = new List<Lawyer> 
        { 
            Lawyer.Create("Andrei Ionescu", "BAR777", "andrei@law.ro") 
        }.AsQueryable();

        repoMock.Setup(x => x.Query()).Returns(data);
        var handler = new GetLawyersQueryHandler(repoMock.Object);

        // Act
        var result = await handler.Handle(new GetLawyersQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        result.First().Name.Should().Be("Andrei Ionescu");
    }
}