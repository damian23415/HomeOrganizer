using AutoMapper;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.Users.Commands;
using HomeOrganizer.Application.Features.Users.DTOs;
using HomeOrganizer.Domain.Interfaces;
using MediatR;

namespace HomeOrganizer.Application.Features.Users.Handlers;

public class LoginUserHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IMapper mapper,
    IJwtTokenGenerator jwtGenerator) : IRequestHandler<LoginUserCommand, Result<LoginUserResponse>>
{
  public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
  {
    var user = await userRepository.GetByEmailAsync(request.Email);
    
    if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
      return Result<LoginUserResponse>.Failure("Invalid email or password.");
    
    var token = jwtGenerator.GenerateToken(user);
    var userDto = mapper.Map<UserDto>(user);
    var response = new LoginUserResponse { User = userDto, Token = token };
    return Result<LoginUserResponse>.Success(response);
  }
}