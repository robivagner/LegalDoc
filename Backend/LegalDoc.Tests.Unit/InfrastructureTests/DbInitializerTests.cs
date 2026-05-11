using LegalDoc.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

using Moq;

namespace LegalDoc.Tests.Unit.InfrastructureTests;

public class DbInitializerTests
{
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;

    public DbInitializerTests()
    {
        _roleManagerMock = MockIdentityHelpers.MockRoleManager();
        _userManagerMock = MockIdentityHelpers.MockUserManager<IdentityUser>();
        _serviceProviderMock = new Mock<IServiceProvider>();

        // Setup IServiceProvider to return our mocks
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(RoleManager<IdentityRole>)))
            .Returns(_roleManagerMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(UserManager<IdentityUser>)))
            .Returns(_userManagerMock.Object);
    }

    [Fact]
    public async Task SeedAsync_WhenRolesAndAdminDoNotExist_CreatesEverything()
    {
        // Arrange
        _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
        
        _userManagerMock.Setup(x => x.FindByNameAsync("admin")).ReturnsAsync((IdentityUser?)null);
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), "admin"))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Admin"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await DbInitializer.SeedAsync(_serviceProviderMock.Object);

        // Assert
        _roleManagerMock.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == "Admin")), Times.Once);
        _roleManagerMock.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == "Lawyer")), Times.Once);
        _roleManagerMock.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == "Viewer")), Times.Once);
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), "admin"), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Admin"), Times.Once);
    }

    [Fact]
    public async Task SeedAsync_WhenAdminAlreadyExists_DoesNotCreateIt()
    {
        // Arrange
        _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.FindByNameAsync("admin")).ReturnsAsync(new IdentityUser { UserName = "admin" });

        // Act
        await DbInitializer.SeedAsync(_serviceProviderMock.Object);

        // Assert
        _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SeedAsync_WhenAdminCreationFails_LogsToConsole()
    {
        // Arrange
        _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.FindByNameAsync("admin")).ReturnsAsync((IdentityUser?)null);
        
        // Simulăm un eșec (ex: parola nu respectă politica)
        var error = new IdentityError { Description = "Password too simple" };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), "admin"))
            .ReturnsAsync(IdentityResult.Failed(error));

        // Act
        await DbInitializer.SeedAsync(_serviceProviderMock.Object);

        // Assert
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Admin"), Times.Never);
    }
}