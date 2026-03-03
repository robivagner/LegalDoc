using FluentValidation;
using LegalDoc.Application.Documents.Commands;

namespace LegalDoc.Application.Documents.Validators;

public class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200).NotEmpty();
        RuleFor(x => x.FileName).MaximumLength(255).Must(fileName => fileName.EndsWith(".pdf") || fileName.EndsWith(".docx")).NotEmpty();
        RuleFor(x => x.StoragePath).NotEmpty();
    }
}