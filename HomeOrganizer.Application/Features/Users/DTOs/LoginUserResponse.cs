namespace HomeOrganizer.Application.Features.Users.DTOs;

public class LoginUserResponse
{
  public string Token { get; set; } = null!;
  public UserDto User { get; set; } = new();
}