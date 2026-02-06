using HomeOrganizer.Application.Features.EmailInterfaces;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using Resend;

namespace HomeOrganizer.Infrastructure.Services;

public class EmailService(IResend resend) : IEmailService
{
  public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
  {
    var message = new EmailMessage
    {
        From = "home.organizer.app@resend.dev",
        To = email,
        Subject = "Potwierdź swój email - HomeOrganizer",
        HtmlBody = $@"
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
            </html>"
        };
    
    await resend.EmailSendAsync(message);
  }
}