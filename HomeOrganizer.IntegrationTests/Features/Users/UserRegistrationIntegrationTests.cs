using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.Users.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HomeOrganizer.IntegrationTests.Features.Users;

[TestFixture]
public class UserRegistrationIntegrationTests
{
  [OneTimeSetUp]
  public void OneTimeSetUp()
  {
    _factory = new WebApplicationFactory<Program>();
  }

  [SetUp]
  public void SetUp()
  {
    _client = _factory.CreateClient();
  }

  [OneTimeTearDown]
  public void OneTimeTearDown()
  {
    _factory?.Dispose();
  }

  [TearDown]
  public void TearDown()
  {
    _client?.Dispose();
  }

  private WebApplicationFactory<Program> _factory;
  private HttpClient _client;

  [Test]
  public async Task RegisterUser_WithValidData_ShouldReturnOkWithTokenAndUser()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = $"test_{Guid.NewGuid()}@example.com",
      Password = "SecurePassword123!"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var result = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
    result.Should().NotBeNull();
    result!.Token.Should().NotBeNullOrEmpty();
    result.User.Should().NotBeNull();
    result.User.Email.Should().Be(request.Email);
    result.User.Role.Should().Be("User");
  }

  [Test]
  public async Task RegisterUser_WithDuplicateEmail_ShouldReturn409Conflict()
  {
    // Arrange
    var email = $"duplicate_{Guid.NewGuid()}@example.com";
    var request = new RegisterUserRequest
    {
      Email = email,
      Password = "SecurePassword123!"
    };

    await _client.PostAsJsonAsync("/api/users/register", request);

    // Act
    var response = await _client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Conflict);

    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    errorResponse.Should().NotBeNull();
    errorResponse!.Message.Should().Contain("błąd");
  }

  [Test]
  public async Task RegisterUser_WithInvalidEmail_ShouldReturn400BadRequest()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = "invalid-email",
      Password = "SecurePassword123!"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    errorResponse.Should().NotBeNull();
    errorResponse!.Message.Should().Be("Błąd walidacji danych");
    errorResponse.Errors.Should().NotBeEmpty();
  }

  [Test]
  public async Task RegisterUser_WithWeakPassword_ShouldReturn400BadRequest()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = "test@example.com",
      Password = "123"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    errorResponse!.Errors.Should().Contain(e => e.Contains("password") || e.Contains("Hasło"));
  }

  [Test]
  public async Task RegisterUser_WithEmptyEmail_ShouldReturn400BadRequest()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = "",
      Password = "SecurePassword123!"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Test]
  public async Task RegisterUser_GeneratedToken_ShouldBeValidJwt()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = $"jwt_test_{Guid.NewGuid()}@example.com",
      Password = "SecurePassword123!"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/users/register", request);
    var result = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();

    // Assert
    var token = result!.Token;
    token.Split('.').Should().HaveCount(3);
  }

  [Test]
  public async Task RegisterUser_ShouldSetDefaultUserRole()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = $"role_test_{Guid.NewGuid()}@example.com",
      Password = "SecurePassword123!"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/users/register", request);
    var result = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();

    // Assert
    result!.User.Role.Should().Be("User");
  }
}