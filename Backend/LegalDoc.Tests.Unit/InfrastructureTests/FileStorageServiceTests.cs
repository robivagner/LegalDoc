using FluentAssertions;
using LegalDoc.Infrastructure.Services;

namespace LegalDoc.Tests.Unit.InfrastructureTests;

public class FileStorageServiceTests : IDisposable
{
    private readonly FileStorageService _sut;
    private readonly string _testDir;

    public FileStorageServiceTests()
    {
        _sut = new FileStorageService();
        // Ne asigurăm că folosim un folder de test care să nu interfereze cu aplicația reală
        _testDir = Path.Combine(Directory.GetCurrentDirectory(), "DocumentsArchive");
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    [Fact]
    public async Task SaveFileAsync_ShouldCreateDirectoryAndReturnPath()
    {
        // Arrange
        var content = "test content"u8.ToArray();
        var fileName = "test.pdf";

        // Act
        var fullPath = await _sut.SaveFileAsync(content, fileName, default);

        // Assert
        File.Exists(fullPath).Should().BeTrue();
        fullPath.Should().Contain("DocumentsArchive");
        fullPath.Should().Contain(fileName);
    }

    [Fact]
    public async Task ReadFileAsync_FileExists_ShouldReturnBytes()
    {
        // Arrange
        var content = "hello world"u8.ToArray();
        var path = await _sut.SaveFileAsync(content, "read-test.txt", default);

        // Act
        var result = await _sut.ReadFileAsync(path, default);

        // Assert
        result.Should().BeEquivalentTo(content);
    }

    [Fact]
    public async Task ReadFileAsync_FileMissing_ShouldThrowFileNotFoundException()
    {
        // Act
        var act = () => _sut.ReadFileAsync("non_existent_path.pdf", default);

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>()
            .WithMessage("Fișierul fizic lipsește de pe server.");
    }
}