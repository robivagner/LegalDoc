namespace LegalDoc.Domain.Enums;

public enum ReviewType
{
    SummaryValidation,    // Verify AI summary
    ClauseExtraction,     // Verify clauses extracted
    RiskAssessment,       // Verify the risk assessments
    FinalApproval         // Final approval
}