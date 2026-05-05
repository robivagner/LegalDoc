using LegalDoc.Application.Abstractions;

namespace LegalDoc.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    public async Task<string> SaveFileAsync(byte[] content, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "DocumentsArchive");
        
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(uploadPath, uniqueFileName);
        
        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);

        return fullPath;
    }
    
    public async Task<byte[]> ReadFileAsync(string storagePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(storagePath) || !File.Exists(storagePath))
        {
            throw new FileNotFoundException("Fișierul fizic lipsește de pe server.");
        }
    
        return await File.ReadAllBytesAsync(storagePath, cancellationToken);
    }
}