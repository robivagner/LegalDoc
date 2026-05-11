using FluentAssertions;
using LegalDoc.Application.Auth.Commands;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LegalDoc.Tests.Unit.AuthTests;

public class AssignRoleTests
{
    [Fact]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        var userManager = MockIdentityHelpers.MockUserManager<IdentityUser>();
        userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null!);
        var handler = new AssignRoleCommandHandler(userManager.Object, MockIdentityHelpers.MockRoleManager<IdentityRole>().Object);

        var act = () => handler.Handle(new AssignRoleCommand("1", "Lawyer"), default);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_InvalidRoleName_ThrowsArgumentException()
    {
        var userManager = MockIdentityHelpers.MockUserManager<IdentityUser>();
        userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser());
        var handler = new AssignRoleCommandHandler(userManager.Object, MockIdentityHelpers.MockRoleManager<IdentityRole>().Object);

        var act = () => handler.Handle(new AssignRoleCommand("1", "Admin"), default);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_ValidRequest_Success()
    {
        var userId = "user-123";
        var user = new IdentityUser { Id = userId };
        var userManager = MockIdentityHelpers.MockUserManager<IdentityUser>();
        var roleManager = MockIdentityHelpers.MockRoleManager<IdentityRole>();
    
        userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        roleManager.Setup(x => x.RoleExistsAsync("Lawyer")).ReturnsAsync(true);
        
        userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        userManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
        userManager.Setup(x => x.AddToRoleAsync(user, "Lawyer")).ReturnsAsync(IdentityResult.Success);

        var handler = new AssignRoleCommandHandler(userManager.Object, roleManager.Object);
        var result = await handler.Handle(new AssignRoleCommand(userId, "Lawyer"), default);

        result.Should().Be(MediatR.Unit.Value);
    }
    
    [Fact]
    public async Task Handle_IdentityFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new IdentityUser { Id = "1" };
        var userManager = MockIdentityHelpers.MockUserManager<IdentityUser>();
        var roleManager = MockIdentityHelpers.MockRoleManager<IdentityRole>();
    
        userManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
        roleManager.Setup(x => x.RoleExistsAsync("Lawyer")).ReturnsAsync(true);
        userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        userManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
    
        // Simulează eșec la adăugarea rolului
        userManager.Setup(x => x.AddToRoleAsync(user, "Lawyer"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Eroare DB" }));

        var handler = new AssignRoleCommandHandler(userManager.Object, roleManager.Object);
    
        // Act
        var act = () => handler.Handle(new AssignRoleCommand("1", "Lawyer"), default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Eroare la asignarea rolului*");
    }
}