using FluentAssertions;
using LegalDoc.Application.Auth.Queries;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LegalDoc.Tests.Unit.AuthTests;

public class GetUsersQueryTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryTests()
    {
        _userManagerMock = MockIdentityHelpers.MockUserManager<IdentityUser>();
        _handler = new GetUsersQueryHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_NoFilter_ReturnsAllUsersWithRoles()
    {
        // Arrange
        var users = new List<IdentityUser>
        {
            new() { Id = "1", UserName = "user1" },
            new() { Id = "2", UserName = "user2" }
        }.AsQueryable();

        // Setup pentru proprietatea .Users
        _userManagerMock.Setup(x => x.Users).Returns(users);
        
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(new List<string> { "Viewer" });

        var query = new GetUsersQuery(null); // Fără filtru

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Should().HaveCount(2);
        result.Any(u => u.UserName == "user1").Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithFilter_ReturnsOnlyMatchingUser()
    {
        // Arrange
        var users = new List<IdentityUser>
        {
            new() { Id = "1", UserName = "target.user" },
            new() { Id = "2", UserName = "other.user" }
        }.AsQueryable();

        _userManagerMock.Setup(x => x.Users).Returns(users);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(new List<string> { "Lawyer" });

        var query = new GetUsersQuery("target.user");

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Should().HaveCount(1);
        result.First().UserName.Should().Be("target.user");
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsEmptyList()
    {
        // Arrange
        var users = new List<IdentityUser>().AsQueryable();
        _userManagerMock.Setup(x => x.Users).Returns(users);

        var query = new GetUsersQuery("non.existent");

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Should().BeEmpty();
    }
}