using System.Security.Cryptography;
using AutoMapper;
using HomeOrganizer.Application.Common.Exceptions;
using HomeOrganizer.Application.Features.EmailInterfaces;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.Users.Dtos;
using HomeOrganizer.Application.Features.Users.Interfaces;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.Users.Handlers;

public class UserRegistrationService(
  IUserRepository userRepository,
  IMapper mapper,
  IPasswordHasher passwordHasher,
  IEmailService emailService,
  IEmailSettings emailSettings)
  : IUserRegistrationService
{
  public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request)
  {
    var existingUser = await userRepository.GetByEmail(request.Email);

    if (existingUser != null)
      throw new UserAlreadyExistsException(request.Email);

    var confirmationToken = GenerateSecureToken();
    
    var user = mapper.Map<User>(request);
    
    user.PasswordHash = passwordHasher.HashPassword(request.Password);
    user.EmailConfirmationToken = confirmationToken;
    user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24);
    
    await userRepository.AddAsync(user);

    var confirmationLink = $"{emailSettings.FrontendUrl}/confirm-email?token={Uri.EscapeDataString(confirmationToken)}&expiry={user.EmailConfirmationTokenExpiry:o}";
    await emailService.SendEmailConfirmationAsync(request.Email, confirmationLink);

    return new RegisterUserResponse
    {
      User = mapper.Map<UserDto>(user)
    };
  }
  
  private string GenerateSecureToken()
  {
    var bytes = new byte[32];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(bytes);
    return Convert.ToBase64String(bytes)
        .Replace("+", "-")
        .Replace("/", "_")
        .Replace("=", "");
  }
}