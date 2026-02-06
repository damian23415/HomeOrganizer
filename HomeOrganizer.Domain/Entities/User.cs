namespace HomeOrganizer.Domain.Entities;

public class User
{
  public Guid Id { get; set; }
  public required string Email { get; set; }
  public required string PasswordHash { get; set; }
  public DateTime Created { get; set; } = DateTime.UtcNow;
  public bool IsActive { get; set; }
  public bool IsEmailConfirmed { get; set; }
  public string? EmailConfirmationToken { get; set; }
  public DateTime? EmailConfirmationTokenExpiry { get; set; }
  public string Role { get; set; } = "User";
}