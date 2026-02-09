namespace HomeOrganizer.Application.Features.EmailInterfaces;

public interface IEmailService
{
  Task SendEmailConfirmationAsync(string receipentEmail, string subject, string body, string confirmationLink);
  string BuildEmailConfirmationBody(string confirmationLink);
  string GetEmailConfirmationSubject();
}