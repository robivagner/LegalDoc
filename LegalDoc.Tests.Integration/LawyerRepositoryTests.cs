using LegalDoc.Infrastructure.Repositories; // Namespace-ul unde ai implementarea
using LegalDoc.Domain.Entities;
using FluentAssertions;

namespace LegalDoc.Tests.Integration;

public class LawyerRepositoryTests
{
    private readonly TestDatabaseFixture _fixture = new();

    [Fact]
    public async Task AddAsync_ShouldPersistLawyerInDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        // Aici pui clasa ta reala de Repository din Infrastructure
        var repository = new LawyerRepository(context); 
        
        var lawyer = Lawyer.Create("Avocat Integrare", "BAR-999", "integrare@test.com");

        // Act
        await repository.AddAsync(lawyer);
        await context.SaveChangesAsync(); // Fortam scrierea pe "disc" (in memorie)

        // Assert
        var savedLawyer = await context.Lawyers.FindAsync(lawyer.Id);
        savedLawyer.Should().NotBeNull();
        savedLawyer!.Name.Should().Be("Avocat Integrare");
        savedLawyer.BarNumber.Should().Be("BAR-999");
    }
}