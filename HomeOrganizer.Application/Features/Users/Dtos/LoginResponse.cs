namespace HomeOrganizer.Application.Features.Users.Dtos;

public class LoginResponse
{
  public string Token { get; set; } = null!;
  public UserDto? User { get; set; } = null!;
}