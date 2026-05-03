using LegalDoc.Infrastructure.Repositories;
using LegalDoc.Application.Document.Commands;
using LegalDoc.Domain.Entities;
using FluentAssertions;
using LegalDoc.Application.Abstractions;
using Moq;

namespace LegalDoc.Tests.Integration;

public class DocumentIntegrationTests
{
    private readonly TestDatabaseFixture _fixture = new();

    [Fact]
    public async Task UploadDocument_ShouldSaveInDatabase_AndLinkToRegistry()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        
        // Cream mai intai un Registru real in DB pentru a avea un ID valid
        var registry = Registry.Create("Arhiva Sud", "Craiova", 50);
        context.Registries.Add(registry);
        await context.SaveChangesAsync();

        var docRepo = new DocumentsRepository(context);
        var regRepo = new RegistryRepository(context);
        var extractorMock = new Mock<IDocumentTextExtractor>();
        var storageServiceMock = new Mock<IFileStorageService>();
        extractorMock
            .Setup(x => x.ExtractTextFromPdf(It.IsAny<string>()))
            .Returns("Text simulat din PDF pentru test");
        storageServiceMock
            .Setup(x => x.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("/simulated/path/contract.pdf");
        var handler = new UploadDocumentCommandHandler(docRepo, regRepo, extractorMock.Object, storageServiceMock.Object);

        var command = new UploadDocumentCommand(
            "Contract Inchiriere", 
            "contract.pdf",
            new byte[] { 1 }, 
            registry.Id);

        // Act
        var docId = await handler.Handle(command, CancellationToken.None);
        await context.SaveChangesAsync();

        // Assert
        var savedDoc = await context.LegalDocuments.FindAsync(docId);
        savedDoc.Should().NotBeNull();
        savedDoc!.RegistryId.Should().Be(registry.Id);
        
        // Verificam daca si disponibilitatea registrului s-a actualizat in DB
        var updatedRegistry = await context.Registries.FindAsync(registry.Id);
        updatedRegistry!.Availability.Should().Be(49);
    }
}