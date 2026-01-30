namespace HomeOrganizer.Application.Features.Users.Dtos;

public class RegisterUserResponse
{
  public string Token { get; set; } = string.Empty;
  public UserDto User { get; set; } = new();
}