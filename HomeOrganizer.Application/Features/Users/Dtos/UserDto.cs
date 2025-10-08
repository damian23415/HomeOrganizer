namespace HomeOrganizer.Application.Features.Users.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public string Role { get; set; } = string.Empty;
}