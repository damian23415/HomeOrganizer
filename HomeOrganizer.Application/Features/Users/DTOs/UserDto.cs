namespace HomeOrganizer.Application.Features.Users.DTOs;

public class UserDto
{
  public Guid Id { get; set; }
  public string Email { get; set; } = null!;
  public bool IsEmailConfirmed { get; set; }
  public bool IsActive { get; set; }
}