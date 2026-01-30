using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Infrastructure.Security;
using NUnit.Framework;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace HomeOrganizer.Tests.Unit.Infrastructure.Security;

[TestFixture]
public class JwtTokenGeneratorTests
{
  [SetUp]
  public void SetUp()
  {
    _jwtSettings = new JwtSettings
    {
      Secret = "super-secret-key-that-is-at-least-32-characters-long",
      Issuer = "HomeOrganizerTest",
      Audience = "HomeOrganizerUsersTest",
      ExpiryMinutes = 60
    };

    _sut = new JwtTokenGenerator(_jwtSettings);
  }

  private JwtSettings? _jwtSettings;
  private JwtTokenGenerator? _sut;

  [Test]
  public void GenerateToken_ShouldReturnValidJwtToken()
  {
    // Arrange
    var user = new User
    {
      Id = Guid.NewGuid(),
      Email = "test@example.com",
      Role = "User",
      PasswordHash = "123"
    };

    // Act
    var token = _sut!.GenerateToken(user);

    // Assert
    token.Should().NotBeNullOrEmpty();

    var parts = token.Split('.');
    parts.Should().HaveCount(3);
  }

  [Test]
  public void GenerateToken_ShouldContainCorrectClaims()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var user = new User
    {
      Id = userId,
      Email = "test@example.com",
      Role = "Admin",
      PasswordHash = "123"
    };

    // Act
    var token = _sut!.GenerateToken(user);

    // Assert
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);

    var subClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);
    var emailClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email);
    var roleClaim = jwtToken.Claims.First(c => c.Type == ClaimTypes.Role);

    subClaim.Value.Should().Be(userId.ToString());
    emailClaim.Value.Should().Be(user.Email);
    roleClaim.Value.Should().Be(user.Role);
  }

  [Test]
  public void GenerateToken_ShouldSetCorrectIssuerAndAudience()
  {
    // Arrange
    var user = new User
    {
      Id = Guid.NewGuid(),
      Email = "test@example.com",
      Role = "User",
      PasswordHash = "123"
    };

    // Act
    var token = _sut!.GenerateToken(user);

    // Assert
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);

    jwtToken.Issuer.Should().Be(_jwtSettings!.Issuer);
    jwtToken.Audiences.Should().Contain(_jwtSettings.Audience);
  }

  [Test]
  public void GenerateToken_ForDifferentUsers_ShouldGenerateDifferentTokens()
  {
    // Arrange
    var user1 = new User
    {
      Id = Guid.NewGuid(),
      Email = "user1@test.com",
      Role = "User",
      PasswordHash = "123"
    };
    var user2 = new User
    {
      Id = Guid.NewGuid(),
      Email = "user2@test.com",
      Role = "Admin",
      PasswordHash = "123"
    };

    // Act
    var token1 = _sut!.GenerateToken(user1);
    var token2 = _sut.GenerateToken(user2);

    // Assert
    token1.Should().NotBe(token2);
  }

  [TestCase("User")]
  [TestCase("Admin")]
  [TestCase("SuperAdmin")]
  public void GenerateToken_ShouldWorkForDifferentRoles(string role)
  {
    // Arrange
    var user = new User
    {
      Id = Guid.NewGuid(),
      Email = "test@example.com",
      Role = role,
      PasswordHash = "123"
    };

    // Act
    var token = _sut!.GenerateToken(user);

    // Assert
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    var roleClaim = jwtToken.Claims.First(c => c.Type == ClaimTypes.Role);

    roleClaim.Value.Should().Be(role);
  }
}