using LegalDoc.Infrastructure.Persistence; // Asigura-te ca acesta e namespace-ul tau
using Microsoft.EntityFrameworkCore;

namespace LegalDoc.Tests.Integration;

public class TestDatabaseFixture
{
    public AppDbContext CreateContext()
    {
        // Cream o baza de date in memorie unica pentru fiecare test
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}