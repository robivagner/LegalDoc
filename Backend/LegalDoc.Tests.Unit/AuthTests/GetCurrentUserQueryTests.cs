using FluentAssertions;
using LegalDoc.Application.Auth.Queries;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LegalDoc.Tests.Unit.AuthTests;

public class GetCurrentUserQueryTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryTests()
    {
        _userManagerMock = MockIdentityHelpers.MockUserManager<IdentityUser>();
        _handler = new GetCurrentUserQueryHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _userManagerMock.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((IdentityUser?)null);

        var query = new GetCurrentUserQuery(userId);

        // Act
        var act = () => _handler.Handle(query, default);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Utilizatorul nu a fost găsit.");
    }

    [Fact]
    public async Task Handle_UserFoundWithRoles_ReturnsDtoWithPrimaryRole()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new IdentityUser { Id = userId, UserName = "robert.vagner" };
        var roles = new List<string> { "Lawyer", "Viewer" };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

        var query = new GetCurrentUserQuery(userId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.UserName.Should().Be("robert.vagner");
        result.Role.Should().Be("Lawyer"); // Primul din listă
    }

    [Fact]
    public async Task Handle_UserFoundWithoutRoles_ReturnsDtoWithDefaultViewerRole()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new IdentityUser { Id = userId, UserName = "new.user" };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>()); // Niciun rol

        var query = new GetCurrentUserQuery(userId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Role.Should().Be("Viewer");
    }
}