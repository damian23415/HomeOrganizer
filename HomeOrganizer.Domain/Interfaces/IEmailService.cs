namespace HomeOrganizer.Domain.Interfaces;

public interface IEmailService
{
  Task SendConfirmationEmailAsync(string email, string token);
}