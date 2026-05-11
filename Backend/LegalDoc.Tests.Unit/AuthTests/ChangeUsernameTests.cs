using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Auth.Commands;
using LegalDoc.Application.Auth.Queries;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LegalDoc.Tests.Unit.AuthTests;

public class ChangeUsernameTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IJwtTokenGenerator> _tokenServiceMock;
    private readonly ChangeUsernameCommandHandler _handler;

    public ChangeUsernameTests()
    {
        _userManagerMock = MockIdentityHelpers.MockUserManager<IdentityUser>();
        _tokenServiceMock = new Mock<IJwtTokenGenerator>();
        _handler = new ChangeUsernameCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((IdentityUser)null!);

        var command = new ChangeUsernameCommand(userId, "new_user");

        // Act
        var act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Utilizatorul nu a fost găsit.");
    }

    [Fact]
    public async Task Handle_UsernameAlreadyTaken_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new IdentityUser { Id = userId.ToString() };
        var existingUser = new IdentityUser { Id = Guid.NewGuid().ToString() }; // Alt ID
    
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.FindByNameAsync("taken_user")).ReturnsAsync(existingUser);

        var command = new ChangeUsernameCommand(userId, "taken_user");

        // Act
        var act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Acest nume de utilizator este deja utilizat.");
    }

    [Fact]
    public async Task Handle_IdentitySetUserNameFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new IdentityUser { Id = userId.ToString() };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null!);
        _userManagerMock.Setup(x => x.SetUserNameAsync(user, "new_user"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Nume invalid" }));

        var command = new ChangeUsernameCommand(userId, "new_user");

        // Act
        var act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Eroare la actualizarea numelui de utilizator: Nume invalid*");
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsAuthResponseWithNewToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new IdentityUser { Id = userId.ToString(), UserName = "old_user" };
        var roles = new List<string> { "Lawyer" };
        var expectedToken = "new-secure-token";
        
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.FindByNameAsync("new_user")).ReturnsAsync((IdentityUser)null!);
        _userManagerMock.Setup(x => x.SetUserNameAsync(user, "new_user")).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
        
        _tokenServiceMock.Setup(x => x.GenerateToken(user, roles)).Returns(expectedToken);

        var command = new ChangeUsernameCommand(userId, "new_user");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.UserName.Should().Be("new_user");
        
        _userManagerMock.Verify(x => x.UpdateNormalizedUserNameAsync(user), Times.Once);
    }
}