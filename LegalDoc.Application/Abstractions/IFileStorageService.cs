namespace LegalDoc.Application.Abstractions;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(byte[] content, string fileName, CancellationToken cancellationToken = default);
    
    Task<byte[]> ReadFileAsync(string storagePath, CancellationToken cancellationToken);
}