using LegalDoc.Application.Lawyer.Commands;
using FluentAssertions;
using LegalDoc.Infrastructure.Repositories;

namespace LegalDoc.Tests.Integration;

public class LawyerWorkflowTests
{
    private readonly TestDatabaseFixture _fixture = new();

    [Fact]
    public async Task CreateLawyerCommand_ShouldActuallySaveToDb()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new LawyerRepository(context);
        var handler = new CreateLawyerCommandHandler(repository);
        
        var command = new CreateLawyerCommand("Integration Handler Test", "123", "h@test.com");

        // Act
        var lawyerId = await handler.Handle(command, CancellationToken.None);
        await context.SaveChangesAsync();

        // Assert
        var exists = context.Lawyers.Any(l => l.Id == lawyerId);
        exists.Should().BeTrue();
    }
}