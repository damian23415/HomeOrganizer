namespace HomeOrganizer.Domain.Interfaces;

public interface IEmailConfirmationTokenGenerator
{
  string GenerateToken();
}