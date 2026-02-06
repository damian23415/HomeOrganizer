using System.Security.Authentication;
using AutoMapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.Users.Dtos;
using HomeOrganizer.Application.Features.Users.Interfaces;

namespace HomeOrganizer.Application.Features.Users.Handlers;

public class UserAuthenticationService(
  IUserRepository userRepository,
  IPasswordHasher passwordHasher,
  IJwtTokenGenerator tokenGenerator,
  IMapper mapper) : IUserAuthenticationService
{
  public async Task<LoginResponse> LoginAsync(LoginRequest request)
  {
    var user = await userRepository.GetByEmail(request.Email);

    if (
        user == null ||
        !user.IsActive ||
        !user.IsEmailConfirmed ||
        !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
    {
      throw new InvalidCredentialException();
    }

    var token = tokenGenerator.GenerateToken(user);

    return new LoginResponse
    {
      Token = token,
      User = mapper.Map<UserDto>(user)
    };
  }
  
  public async Task ConfirmEmailAsync(string confirmationToken, DateTime expirationDate)
  {
    var user = await userRepository.GetUserByTokenAsync(confirmationToken);

    if (user == null)
    {
      throw new InvalidOperationException("Invalid token");
    }

    if (user.EmailConfirmationTokenExpiry < expirationDate)
    {
      throw new InvalidOperationException("Token has expired");
    }

    user.IsActive = true;
    user.IsEmailConfirmed = true;
    user.EmailConfirmationToken = null;
    user.EmailConfirmationTokenExpiry = null;

    await userRepository.UpdateAsync(user);
  }
}