using LegalDoc.Application.Document.Queries;

namespace LegalDoc.Application.Abstractions;

public interface IAiService
{
    Task<AiAnalysisResponse?> AnalyzeDocumentAsync(string text);
}