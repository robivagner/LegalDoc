using FluentAssertions;
using LegalDoc.Domain.Entities;

namespace LegalDoc.Tests.Unit.DocumentsTests;

public class CreateLegalDocumentTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_InvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        var act = () => LegalDocument.Create(invalidTitle!, "file.pdf", "/path", "content", Guid.NewGuid());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Document title cannot be null or empty.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_InvalidFileName_ThrowsArgumentException(string? invalidFileName)
    {
        var act = () => LegalDocument.Create("Titlu", invalidFileName!, "/path", "content", Guid.NewGuid());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Document file name cannot be null or empty.*");
    }

    [Fact]
    public void Create_ValidData_ReturnsLegalDocument()
    {
        var registryId = Guid.NewGuid();
        var result = LegalDocument.Create("Contract", "c.pdf", "/store", "text", registryId);

        result.Should().NotBeNull();
        result.Title.Should().Be("Contract");
        result.RegistryId.Should().Be(registryId);
        result.Status.ToString().Should().Be("Uploaded"); // Verificăm statusul inițial
    }
}