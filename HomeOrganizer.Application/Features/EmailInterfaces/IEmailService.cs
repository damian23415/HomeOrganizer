namespace HomeOrganizer.Application.Features.EmailInterfaces;

public interface IEmailService
{
  Task SendEmailConfirmationAsync(string email, string confirmationLink);
}