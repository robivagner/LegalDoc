using FluentAssertions;
using LegalDoc.Application.Auth.Commands;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LegalDoc.Tests.Unit.AuthTests;

public class RegisterTests
{
    [Fact]
    public async Task Handle_RegistrationFails_ThrowsInvalidOperationException()
    {
        var userManager = MockIdentityHelpers.MockUserManager<IdentityUser>();
        userManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), "pass"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User already exists" }));

        var handler = new RegisterCommandHandler(userManager.Object);
        var act = () => handler.Handle(new RegisterCommand("user", "pass"), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Registration failed*");
    }
}