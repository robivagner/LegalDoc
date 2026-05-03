using FluentValidation;
using LegalDoc.Application.Document.Commands;

namespace LegalDoc.Application.Document.Validators;

public class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200).NotEmpty();
        RuleFor(x => x.FileName).MaximumLength(255).Must(fileName => fileName.EndsWith(".pdf")).NotEmpty();
        RuleFor(x => x.FileContent)
            .NotNull()
            .NotEmpty().WithMessage("Fișierul nu poate fi gol.");

        RuleFor(x => x.RegistryId)
            .NotEmpty().WithMessage("Trebuie să selectezi un registru valid.");
    }
}