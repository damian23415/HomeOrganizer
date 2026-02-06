using AutoMapper;
using FluentAssertions;
using HomeOrganizer.Application.Common.Exceptions;
using HomeOrganizer.Application.Features.EmailInterfaces;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.Users.Dtos;
using HomeOrganizer.Application.Features.Users.Handlers;
using HomeOrganizer.Domain.Entities;
using Moq;
using NUnit.Framework;

namespace HomeOrganizer.Tests.Unit.Features.Users;

[TestFixture]
public class UserRegistrationServiceTests
{
  [SetUp]
  public void SetUp()
  {
    _userRepositoryMock = new Mock<IUserRepository>();
    _mapperMock = new Mock<IMapper>();
    _passwordHasherMock = new Mock<IPasswordHasher>();
    _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
    _emailService = new Mock<IEmailService>();
    _emailSettings = new Mock<IEmailSettings>();

    _sut = new UserRegistrationService(
      _userRepositoryMock.Object,
      _mapperMock.Object,
      _passwordHasherMock.Object,
      _emailService.Object,
      _emailSettings.Object
    );
  }

  private Mock<IUserRepository>? _userRepositoryMock;
  private Mock<IMapper>? _mapperMock;
  private Mock<IPasswordHasher>? _passwordHasherMock;
  private Mock<IJwtTokenGenerator>? _jwtTokenGeneratorMock;
  private UserRegistrationService? _sut;
  private Mock<IEmailService>? _emailService;
  private Mock<IEmailSettings>? _emailSettings;

  [Test]
  public async Task RegisterAsync_WhenUserDoesNotExist_ShouldRegisterSuccessfully()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = "test@example.com",
      Password = "SecurePassword123!"
    };

    var user = new User
    {
      Id = Guid.NewGuid(),
      Email = request.Email,
      PasswordHash = "hashed_password",
      Created = DateTime.UtcNow,
      IsActive = true,
      Role = "User"
    };

    var userDto = new UserDto
    {
      Id = user.Id,
      Email = user.Email,
      Role = user.Role
    };

    const string expectedToken = "jwt_token_here";

    _userRepositoryMock
      .Setup(x => x.GetByEmail(request.Email))
      .ReturnsAsync((User?)null);

    _mapperMock
      .Setup(x => x.Map<User>(request))
      .Returns(user);

    _passwordHasherMock
      .Setup(x => x.HashPassword(request.Password))
      .Returns("hashed_password");

    _jwtTokenGeneratorMock
      .Setup(x => x.GenerateToken(user))
      .Returns(expectedToken);

    _mapperMock
      .Setup(x => x.Map<UserDto>(user))
      .Returns(userDto);

    // Act
    var result = await _sut.RegisterAsync(request);

    // Assert
    result.Should().NotBeNull();
    result.User.Should().BeEquivalentTo(userDto);

    _userRepositoryMock.Verify(x => x.GetByEmail(request.Email), Times.Once);
    _passwordHasherMock.Verify(x => x.HashPassword(request.Password), Times.Once);
    _jwtTokenGeneratorMock.Verify(x => x.GenerateToken(user), Times.Once);
    _userRepositoryMock.Verify(x => x.AddAsync(user), Times.Once);
  }

  [Test]
  public async Task RegisterAsync_WhenUserAlreadyExists_ShouldThrowUserAlreadyExistsException()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = "existing@example.com",
      Password = "SecurePassword123!"
    };

    var existingUser = new User
    {
      Id = Guid.NewGuid(),
      Email = request.Email,
      PasswordHash = "old_hash",
      Created = DateTime.UtcNow.AddDays(-10),
      IsActive = true,
      Role = "User"
    };

    _userRepositoryMock
      .Setup(x => x.GetByEmail(request.Email))
      .ReturnsAsync(existingUser);

    // Act
    var act = async () => await _sut.RegisterAsync(request);

    // Assert
    await act.Should().ThrowAsync<UserAlreadyExistsException>()
      .WithMessage($"User with email {request.Email} already exists.");

    _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
  }

  [Test]
  public async Task RegisterAsync_ShouldHashPasswordBeforeSaving()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      Email = "test@example.com",
      Password = "PlainPassword123"
    };

    var user = new User
    {
      Email = request.Email,
      PasswordHash = "123"
    };
    const string hashedPassword = "hashed_password_xyz";

    _userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync((User?)null);
    _mapperMock.Setup(x => x.Map<User>(request)).Returns(user);
    _passwordHasherMock.Setup(x => x.HashPassword(request.Password)).Returns(hashedPassword);
    _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("token");
    _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto());

    // Act
    await _sut.RegisterAsync(request);

    // Assert
    _passwordHasherMock.Verify(x => x.HashPassword(request.Password), Times.Once);
    user.PasswordHash.Should().Be(hashedPassword);
  }

  [Test]
  public async Task RegisterAsync_ShouldGenerateTokenBeforeSavingToDatabase()
  {
    // Arrange
    var request = new RegisterUserRequest { Email = "test@example.com", Password = "Pass123" };
    var user = new User
    {
      Email = request.Email,
      PasswordHash = "123"
    };
    var callOrder = new List<string>();

    _userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync((User?)null);
    _mapperMock.Setup(x => x.Map<User>(request)).Returns(user);
    _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hash");

    _jwtTokenGeneratorMock
      .Setup(x => x.GenerateToken(It.IsAny<User>()))
      .Returns("token")
      .Callback(() => callOrder.Add("GenerateToken"));

    _userRepositoryMock
      .Setup(x => x.AddAsync(It.IsAny<User>()))
      .Callback(() => callOrder.Add("AddAsync"))
      .Returns(Task.CompletedTask);

    _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto());

    // Act
    await _sut.RegisterAsync(request);

    // Assert
    callOrder.Should().ContainInOrder("GenerateToken", "AddAsync");
  }
}