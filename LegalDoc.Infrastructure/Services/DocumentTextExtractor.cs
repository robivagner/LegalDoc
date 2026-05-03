using System.Text;
using LegalDoc.Application.Abstractions;
using UglyToad.PdfPig;

namespace LegalDoc.Infrastructure.Services;

public class DocumentTextExtractor : IDocumentTextExtractor
{
    public string ExtractTextFromPdf(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("PDF file not found in the given path.", filePath);
        
        var sb = new StringBuilder();
        using (var pdf = PdfDocument.Open(filePath))
        {
            foreach (var page in pdf.GetPages())
            {
                sb.Append(page.Text);
                sb.Append(' ');
            }
        }
        return sb.ToString().Trim();
    }
}