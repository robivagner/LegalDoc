namespace LegalDoc.Application.Abstractions;

public interface IDocumentTextExtractor
{
    public string ExtractTextFromPdf(string filePath);
}