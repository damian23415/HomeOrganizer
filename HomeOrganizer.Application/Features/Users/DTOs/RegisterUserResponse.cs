using HomeOrganizer.Application.Features.Users.DTOs;

namespace HomeOrganizer.Application.DTOs.Users;

public class RegisterUserResponse
{
  public UserDto User { get; set; } = new();
}