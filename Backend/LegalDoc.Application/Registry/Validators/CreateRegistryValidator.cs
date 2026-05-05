using FluentValidation;
using LegalDoc.Application.Registry.Commands;

namespace LegalDoc.Application.Registry.Validators;

public class CreateRegistryValidator : AbstractValidator<CreateRegistryCommand>
{
    public CreateRegistryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("Capacitatea trebuie sa fie cel putin 1.");
    }
}