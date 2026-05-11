using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Document.Queries;
using LegalDoc.Domain.Entities;
using Moq;

namespace LegalDoc.Tests.Unit.DocumentsTests;

public class GetDocumentFileQueryTests
{
    private readonly Mock<IDocumentsRepository> _docRepoMock = new();
    private readonly Mock<IFileStorageService> _fileStorageMock = new();

    [Fact]
    public async Task Handle_DocumentNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var docId = Guid.NewGuid();
        _docRepoMock.Setup(x => x.FindAsync(docId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LegalDocument?)null);

        var handler = new GetDocumentFileQueryHandler(_docRepoMock.Object, _fileStorageMock.Object);
        var query = new GetDocumentFileQuery(docId);

        // Act
        var act = () => handler.Handle(query, default);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Document negăsit.");
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsDocumentFileDto()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var storagePath = "/uploads/contract.pdf";
        var fileName = "contract.pdf";
        var document = LegalDocument.Create("Titlu", fileName, storagePath, "continut", Guid.NewGuid());
        
        var fakeFileContent = new byte[] { 0x20, 0x20, 0x20, 0x20 }; // Simulare bytes PDF

        _docRepoMock.Setup(x => x.FindAsync(docId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);
        
        _fileStorageMock.Setup(x => x.ReadFileAsync(storagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeFileContent);

        var handler = new GetDocumentFileQueryHandler(_docRepoMock.Object, _fileStorageMock.Object);
        var query = new GetDocumentFileQuery(docId);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Content.Should().BeEquivalentTo(fakeFileContent);
        result.ContentType.Should().Be("application/pdf");
        
        _fileStorageMock.Verify(x => x.ReadFileAsync(storagePath, It.IsAny<CancellationToken>()), Times.Once);
    }
}