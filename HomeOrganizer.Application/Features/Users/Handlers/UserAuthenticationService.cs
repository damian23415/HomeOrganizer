using System.Security.Authentication;
using AutoMapper;
using HomeOrganizer.Application.Features.Repositories;
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

    if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
      throw new InvalidCredentialException();

    var token = tokenGenerator.GenerateToken(user);

    return new LoginResponse
    {
      Token = token,
      User = mapper.Map<UserDto>(user)
    };
  }
}