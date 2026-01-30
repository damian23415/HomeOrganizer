namespace HomeOrganizer.Domain.Entities;

public class User
{
  public Guid Id { get; set; }
  public required string Email { get; set; }
  public required string PasswordHash { get; set; }
  public DateTime Created { get; set; } = DateTime.UtcNow;
  public bool IsActive { get; set; } = true;
  public string Role { get; set; } = "User";
}