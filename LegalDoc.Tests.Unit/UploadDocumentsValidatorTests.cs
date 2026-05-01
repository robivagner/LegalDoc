using FluentValidation.TestHelper;
using LegalDoc.Application.Document.Commands;
using LegalDoc.Application.Document.Validators;

namespace LegalDoc.Tests.Unit;

public class UploadDocumentValidatorTests
{
    private readonly UploadDocumentValidator _validator = new();

    [Theory]
    [InlineData("document.pdf")]
    [InlineData("contract.docx")]
    public void Validator_Should_NotHaveError_WhenExtensionIsValid(string fileName)
    {
        // Arrange
        var command = new UploadDocumentCommand("Titlu", fileName, "/path", "Content", Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FileName);
    }

    [Theory]
    [InlineData("document.exe")]
    [InlineData("imagine.png")]
    [InlineData("text.txt")]
    public void Validator_Should_HaveError_WhenExtensionIsInvalid(string fileName)
    {
        // Arrange
        var command = new UploadDocumentCommand("Titlu", fileName, "/path", "Content", Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert - Verificam daca validatorul opreste fisierele care nu sunt PDF sau DOCX
        result.ShouldHaveValidationErrorFor(x => x.FileName);
    }
}