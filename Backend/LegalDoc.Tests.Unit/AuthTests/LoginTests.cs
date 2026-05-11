using FluentAssertions;
using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Auth.Commands;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LegalDoc.Tests.Unit.AuthTests;

public class LoginTests
{
    [Fact]
    public async Task Handle_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        var userManager = MockIdentityHelpers.MockUserManager<IdentityUser>();
        userManager.Setup(x => x.FindByNameAsync("user")).ReturnsAsync(new IdentityUser());
        userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), "wrong")).ReturnsAsync(false);

        var handler = new LoginCommandHandler(userManager.Object, Mock.Of<IJwtTokenGenerator>());
        var act = () => handler.Handle(new LoginCommand("user", "wrong"), default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_SuccessfulLogin_ReturnsAuthResponse()
    {
        var user = new IdentityUser { UserName = "robert" };
        var userManager = MockIdentityHelpers.MockUserManager<IdentityUser>();
        var tokenGen = new Mock<IJwtTokenGenerator>();

        userManager.Setup(x => x.FindByNameAsync("robert")).ReturnsAsync(user);
        userManager.Setup(x => x.CheckPasswordAsync(user, "pass123")).ReturnsAsync(true);
        userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Lawyer" });
        tokenGen.Setup(x => x.GenerateToken(user, It.IsAny<IList<string>>())).Returns("fake-jwt-token");

        var handler = new LoginCommandHandler(userManager.Object, tokenGen.Object);
        var response = await handler.Handle(new LoginCommand("robert", "pass123"), default);

        response.Token.Should().Be("fake-jwt-token");
        response.UserName.Should().Be("robert");
    }
}