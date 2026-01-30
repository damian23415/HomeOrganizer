namespace HomeOrganizer.Application.Features.Users.Dtos;

public class LoginRequest
{
  public string Email { get; set; } = null!;
  public string Password { get; set; } = null!;
}