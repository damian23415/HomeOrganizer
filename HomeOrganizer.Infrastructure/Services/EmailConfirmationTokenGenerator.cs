using HomeOrganizer.Domain.Interfaces;

namespace HomeOrganizer.Infrastructure.Services;

public class EmailConfirmationTokenGenerator : IEmailConfirmationTokenGenerator
{
  public string GenerateToken() => Guid.NewGuid().ToString();
}