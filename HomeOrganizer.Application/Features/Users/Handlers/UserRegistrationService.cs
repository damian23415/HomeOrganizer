using AutoMapper;
using HomeOrganizer.Application.Common.Exceptions;
using HomeOrganizer.Application.Features.Repositories;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.Users.Dtos;
using HomeOrganizer.Application.Features.Users.Interfaces;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.Users.Handlers;

public class UserRegistrationService(
    IUserRepository userRepository,
    IMapper mapper,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
    : IUserRegistrationService
{
    public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request)
    {
        var existingUser = await userRepository.GetByEmail(request.Email);

        if (existingUser != null)
            throw new UserAlreadyExistsException(request.Email);
        
        var user = mapper.Map<User>(request);
        user.PasswordHash = passwordHasher.HashPassword(request.Password);

        var token = jwtTokenGenerator.GenerateToken(user);
        
        await userRepository.AddAsync(user);

        return new RegisterUserResponse()
        {
            Token = token,
            User = mapper.Map<UserDto>(user)
        };
    }
}