using FluentAssertions;
using LegalDoc.Application.Auth.Commands;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LegalDoc.Tests.Unit.AuthTests;

public class ChangePasswordTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordTests()
    {
        _userManagerMock = MockIdentityHelpers.MockUserManager<IdentityUser>();
        _handler = new ChangePasswordCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((IdentityUser?)null);

        var command = new ChangePasswordCommand(Guid.NewGuid(), "old", "new");

        // Act
        var act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Utilizatorul nu a fost găsit.");
    }

    [Fact]
    public async Task Handle_WrongCurrentPassword_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new IdentityUser { Id = userId.ToString() };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        
        // Simulăm eroarea de PasswordMismatch returnată de Identity
        var identityError = new IdentityError { 
            Code = "PasswordMismatch", 
            Description = "Parola actuală este incorectă." 
        };
        
        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "wrong_old", "new"))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var command = new ChangePasswordCommand(userId, "wrong_old", "new");

        // Act
        var act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Parola actuală este incorectă*");
    }

    [Fact]
    public async Task Handle_IdentityError_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new IdentityUser();
        _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "old", "new"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Parola prea simplă" }));

        var command = new ChangePasswordCommand(Guid.NewGuid(), "old", "new");

        // Act
        var act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Parola prea simplă*");
    }

    [Fact]
    public async Task Handle_ValidRequest_Succeeds()
    {
        // Arrange
        var user = new IdentityUser { Id = Guid.NewGuid().ToString() };
        _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "old", "new"))
            .ReturnsAsync(IdentityResult.Success);

        var command = new ChangePasswordCommand(Guid.Parse(user.Id), "old", "new");

        // Act
        await _handler.Handle(command, default);

        // Assert
        // Verificăm dacă metoda din UserManager a fost apelată efectiv
        _userManagerMock.Verify(x => x.ChangePasswordAsync(user, "old", "new"), Times.Once);
    }
}