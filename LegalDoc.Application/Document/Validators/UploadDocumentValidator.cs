using FluentValidation;
using LegalDoc.Application.Document.Commands;

namespace LegalDoc.Application.Document.Validators;

public class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200).NotEmpty();
        RuleFor(x => x.FileName).MaximumLength(255).Must(fileName => fileName.EndsWith(".pdf") || fileName.EndsWith(".docx")).NotEmpty();
        RuleFor(x => x.StoragePath).NotEmpty();
    }
}