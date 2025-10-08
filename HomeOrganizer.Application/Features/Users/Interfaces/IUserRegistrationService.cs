using HomeOrganizer.Application.Features.Users.Dtos;

namespace HomeOrganizer.Application.Features.Users.Interfaces;

public interface IUserRegistrationService
{
    Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request);
}