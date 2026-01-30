using HomeOrganizer.Application.Features.Users.Dtos;

namespace HomeOrganizer.Application.Features.Users.Interfaces;

public interface IUserAuthenticationService
{
  Task<LoginResponse> LoginAsync(LoginRequest request);
}