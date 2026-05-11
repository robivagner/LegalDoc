using FluentAssertions;
using LegalDoc.Infrastructure.Services;

namespace LegalDoc.Tests.Unit.InfrastructureTests;

public class DocumentTextExtractorTests
{
    private readonly DocumentTextExtractor _sut = new();

    [Fact]
    public void ExtractTextFromPdf_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        // Act
        var act = () => _sut.ExtractTextFromPdf("invalid_path_123.pdf");

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage("PDF file not found in the given path.");
    }
    
    [Fact]
    public void ExtractTextFromPdf_ValidFile_ReturnsText()
    {
        string tinyPdfBase64 = "JVBERi0xLjEKMSAwIG9iajw8L1R5cGUvQ2F0YWxvZy9QYWdlcyAyIDAgUj4+ZW5kb2JqIDIgMCBvYmo8PC9UeXBlL1BhZ2VzL0tpZHNbMyAwIFJdL0NvdW50IDE+PmVuZG9iaiAzIDAgb2JqPDwvVHlwZS9QYWdlL1BhcmVudCAyIDAgUi9NZWRpYUJveFswIDAgNjEyIDc5Ml0vUmVzb3VyY2VzPDw+Pi9Db250ZW50cyA0IDAgUj4+ZW5kb2JqIDQgMCBvYmo8PC9MZW5ndGggMjE+PnN0cmVhbQpCVCAvRjEgMTIgVGYgMCAwIFRkIEVUCmVuZHN0cmVhbSBlbmRvYmoKeHJlZgowIDUKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDEwIDAwMDAwIG4gCjAwMDAwMDAwNTkgMDAwMDAgbiAKMDAwMDAwMDExNiAwMDAwMCBuIAowMDAwMDAwMjIzIDAwMDAwIG4gCnRyYWlsZXI8PC9TaXplIDUvUm9vdCAxIDAgUj4+CnN0YXJ0eHJlZgoyOTMKJSVFT0Y=";
    
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, Convert.FromBase64String(tinyPdfBase64));

        try
        {
            // Act
            var result = _sut.ExtractTextFromPdf(path);

            // Assert
            result.Should().Be(""); 
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}