using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using LegalDoc.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace LegalDoc.Tests.Unit.InfrastructureTests;

public class JwtTokenGeneratorTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly JwtTokenGenerator _sut;

    public JwtTokenGeneratorTests()
    {
        _configMock = new Mock<IConfiguration>();
        
        // Cheia trebuie să fie lungă (peste 32 caractere) pentru HS256
        string testSecret = "cheie_secreta_foarte_lunga_si_sigura_123456_pentru_unit_tests";
        
        // REZOLVARE 1: Mapăm exact cheia pe care o caută codul tău: "JwtSettings:Secret"
        _configMock.Setup(x => x["JwtSettings:Secret"]).Returns(testSecret);
        
        // Setăm variabila de mediu pentru a fi siguri că trece de throw
        Environment.SetEnvironmentVariable("JWT_SECRET", testSecret); 

        _sut = new JwtTokenGenerator(_configMock.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtString()
    {
        // Arrange
        var user = new IdentityUser { Id = "user-123", UserName = "robert.vagner", Email = "robert@test.com" };
        var roles = new List<string> { "Lawyer" };

        // Act
        var token = _sut.GenerateToken(user, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();
    
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // REZOLVARE: Verificăm numele scurte pe care JwtSecurityTokenHandler le produce efectiv
        jwtToken.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == "user-123");
        jwtToken.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == "robert.vagner");
        jwtToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == "robert@test.com");
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Lawyer");
    }
}