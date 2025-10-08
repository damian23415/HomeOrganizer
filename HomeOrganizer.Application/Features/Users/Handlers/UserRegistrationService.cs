using AutoMapper;
using HomeOrganizer.Application.Features.Repositories;
using HomeOrganizer.Application.Features.Users.Dtos;
using HomeOrganizer.Application.Features.Users.Interfaces;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.Users.Handlers;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UserRegistrationService(IUserRepository userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request)
    {
        var existingUser = await _userRepository.GetByEmail(request.Email);
        
        if (existingUser != null)
            throw new Exception($"User with email {request.Email} already exists");
        
        var user = _mapper.Map<User>(request);
        user.PasswordHash = _passwordHasher.HashPassword(request.Password);

        await _userRepository.AddAsync(user);
        
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new RegisterUserResponse()
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        };
    }
}