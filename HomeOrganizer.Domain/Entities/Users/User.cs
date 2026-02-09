using HomeOrganizer.Domain.Enums;
using HomeOrganizer.Domain.Exceptions;
using HomeOrganizer.Domain.ValueObjects;

namespace HomeOrganizer.Domain.Entities.Users;

public class User : BaseEntity
{
  public EmailAddress Email { get; private set; }
  public UserRole Role { get; private set; }

  public string PasswordHash { get; private set; }
  public bool IsActive { get; set; }
  public bool IsEmailConfirmed { get; set; }
  public string? EmailConfirmationToken { get; set; }
  public DateTime? EmailConfirmationTokenExpiry { get; set; }
  
  private User() { } // For EF Core
  
  public User(EmailAddress email, string passwordHash, UserRole? role = null)
  {
    Email = email ?? throw new ArgumentNullException(nameof(email));
    Role = role ?? UserRole.User();
    
    if (string.IsNullOrWhiteSpace(passwordHash))
      throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
    
    PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
  }
  
  public void ConfirmEmail(string token)
  {
    if (string.IsNullOrWhiteSpace(token))
      throw new UserDomainException("Token cannot be empty.");
        
    if (IsEmailConfirmed)
      throw new UserDomainException("Email already confirmed.");
        
    if (EmailConfirmationToken != token || EmailConfirmationTokenExpiry <= DateTime.UtcNow)
      throw new UserDomainException("Invalid or expired token.");
        
    IsEmailConfirmed = true;
    EmailConfirmationToken = null;
    EmailConfirmationTokenExpiry = null;
  }
  
  public void SetEmailConfirmationToken(string token, DateTime expiry)
  {
    EmailConfirmationToken = token;
    EmailConfirmationTokenExpiry = expiry;
  }
  
  public void Deactivate() => IsActive = false;
  public bool IsAdmin() => Role.Value == UserRoleEnum.Admin;

}