namespace LegalDoc.Application.Document.Queries;

public record AiAnalysisRequest(string Content);

public record AiAnalysisResponse(
    string Summary,
    string Clauses,
    string Risks
);