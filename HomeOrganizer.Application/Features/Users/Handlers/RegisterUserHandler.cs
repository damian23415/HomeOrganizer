using AutoMapper;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.DTOs.Users;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.Users.Commands;
using HomeOrganizer.Application.Features.Users.DTOs;
using HomeOrganizer.Domain.Entities.Users;
using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Domain.ValueObjects;
using MediatR;

namespace HomeOrganizer.Application.Features.Users.Handlers;

public class RegisterUserHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IMapper mapper) : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
  public async Task<Result<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
  {
    var exising = await userRepository.GetByEmailAsync(request.Email);
    
    if (exising != null)
      return Result<RegisterUserResponse>.Failure("User already exists.");
    
    var hashedPassword = passwordHasher.HashPassword(request.Password);
    var user = new User(new EmailAddress(request.Email), hashedPassword);
    
    await userRepository.AddAsync(user);

    var userDto = mapper.Map<UserDto>(user);
    var response = new RegisterUserResponse { User = userDto };

    return Result<RegisterUserResponse>.Success(response);
  }
}