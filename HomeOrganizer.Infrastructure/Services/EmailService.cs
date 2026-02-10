using MailKit.Net.Smtp;
using HomeOrganizer.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace HomeOrganizer.Infrastructure.Services;

public class EmailService(IConfiguration config) : IEmailService
{
  public async Task SendConfirmationEmailAsync(string email, string token)
  {
      var baseUrl = config["App:BaseUrl"];
      var expiry = DateTime.UtcNow.AddHours(24);
      var confirmUrl = $"{baseUrl}/confirm-email?token={token}&expiry={expiry:o}";
      
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress("HomeOrganizer", config["Smtp:FromEmail"]!));
      message.To.Add(new MailboxAddress("", email));
      message.Subject = "Potwierdź swój email";
      
      message.Body = new TextPart("plain") { Text = BuildEmailConfirmationBody(confirmUrl) };

      using var client = new SmtpClient();
      
      await client.ConnectAsync(config["Smtp:Host"]!, 587, MailKit.Security.SecureSocketOptions.StartTls);
      await client.AuthenticateAsync(config["Smtp:Username"]!, config["Smtp:Password"]!);
      await client.SendAsync(message);
      await client.DisconnectAsync(true);
  }

  private string BuildEmailConfirmationBody(string confirmationLink)
  {
      return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ 
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white;
                        padding: 30px;
                        text-align: center;
                        border-radius: 10px 10px 0 0;
                    }}
                    .content {{ 
                        background: #f8f9fa;
                        padding: 30px;
                        border-radius: 0 0 10px 10px;
                    }}
                    .button {{ 
                        display: inline-block;
                        padding: 15px 30px;
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white !important;
                        text-decoration: none;
                        border-radius: 8px;
                        font-weight: bold;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>🏠 Witaj w HomeOrganizer!</h1>
                    </div>
                    <div class='content'>
                        <p>Dziękujemy za rejestrację!</p>
                        <p>Aby aktywować swoje konto, kliknij w poniższy przycisk:</p>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='{confirmationLink}' class='button'>Potwierdź email</a>
                        </p>
                        <p>Lub skopiuj link:</p>
                        <p style='word-break: break-all;'>{confirmationLink}</p>
                    </div>
                </div>
            </body>
            </html>";
  }
}