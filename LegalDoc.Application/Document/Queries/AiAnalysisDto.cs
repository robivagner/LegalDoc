namespace LegalDoc.Application.Document.Queries;

public record AiAnalysisRequest(string Text);

public record AiAnalysisResponse(
    string Summary,
    string Clauses,
    string Risks
);