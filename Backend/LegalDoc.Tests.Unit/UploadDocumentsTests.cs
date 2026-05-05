using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Document.Commands;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;
using LegalDoc.Infrastructure.Services;

namespace LegalDoc.Tests.Unit;

public class UploadDocumentTests
{
    private readonly Mock<IDocumentsRepository> _docRepoMock;
    private readonly Mock<IRegistryRepository> _registryRepoMock;
    private readonly Mock<IDocumentTextExtractor> _extractorMock;
    private readonly Mock<IFileStorageService> _storageServiceMock;
    private readonly UploadDocumentCommandHandler _handler;

    public UploadDocumentTests()
    {
        // 1. Simulam interfețele de repository folosind Moq
        _docRepoMock = new Mock<IDocumentsRepository>();
        _registryRepoMock = new Mock<IRegistryRepository>();
        _extractorMock = new Mock<IDocumentTextExtractor>();
        _storageServiceMock = new Mock<IFileStorageService>();

        // 2. Initializam Handler-ul cu obiectele simulate (Mocks)
        _handler = new UploadDocumentCommandHandler(_docRepoMock.Object, _registryRepoMock.Object, _extractorMock.Object, _storageServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenRegistryDoesNotExist()
    {
        // Arrange - Pregatim datele de test
        var command = new UploadDocumentCommand("Titlu", "file.pdf", new byte[] { 1 }, Guid.NewGuid());
        
        // Simulam faptul ca FindAsync returneaza null (registrul nu e gasit)
        _registryRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Registry)null!);

        // Act - Executam actiunea
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert - Verificam daca a aruncat exceptia corecta
        await act.Should().ThrowAsync<Exception>().WithMessage("Registry not found!");
    }

    [Fact]
    public async Task Handle_Should_CreateDocument_WhenRegistryIsValid()
    {
        // Arrange - Cream un registru valid pentru test
        var registryId = Guid.NewGuid();
        var command = new UploadDocumentCommand("Contract", "document1.pdf", new byte[] {1}, registryId);
        
        var registry = Registry.Create("Arhiva Centrala", "Bucuresti", 100);
        
        _registryRepoMock
            .Setup(r => r.FindAsync(registryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(registry);
        
        _storageServiceMock
            .Setup(x => x.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("/fake/path.pdf");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty(); // Trebuie sa primim un ID de document
        
        // Verificam daca metoda AddAsync a fost apelata exact o data
        _docRepoMock.Verify(x => x.AddAsync(It.IsAny<LegalDocument>(), It.IsAny<CancellationToken>()), Times.Once);
        
        // Verificam logica de business: capacitatea registrului trebuie sa scada cu 1
        registry.Availability.Should().Be(99);
    }
    
    [Fact]
    public async Task Handle_Should_ThrowException_WhenRegistryIdIsInvalid()
    {
        // Arrange
        _registryRepoMock.Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Registry)null!);

        var command = new UploadDocumentCommand("Titlu", "f.pdf", new byte[] {1}, Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Registry not found!");
    }
}