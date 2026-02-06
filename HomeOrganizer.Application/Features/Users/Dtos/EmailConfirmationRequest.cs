namespace HomeOrganizer.Application.Features.Users.Dtos;

public class EmailConfirmationRequest
{
  public string Token { get; set; } = string.Empty; 
  public DateTime EmailConfirmationTokenExpiry { get; set; }
}