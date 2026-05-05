using FluentValidation.TestHelper;
using LegalDoc.Application.Registry.Commands;
using LegalDoc.Application.Registry.Validators;

namespace LegalDoc.Tests.Unit;

public class CreateRegistryValidatorTests
{
    private readonly CreateRegistryValidator _validator = new();

    [Fact]
    public void Validator_Should_HaveError_WhenCapacityIsZeroOrLess()
    {
        // Arrange
        var command = new CreateRegistryCommand("Test", "Locatie", 0);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Capacity)
            .WithErrorMessage("Capacitatea trebuie sa fie cel putin 1.");
    }

    [Fact]
    public void Validator_Should_NotHaveError_WhenDataIsValid()
    {
        // Arrange
        var command = new CreateRegistryCommand("Arhiva", "Bucuresti", 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}