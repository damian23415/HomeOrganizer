using HomeOrganizer.Application.Features.EmailInterfaces;

namespace HomeOrganizer.Infrastructure.Settings;

public class EmailSettings : IEmailSettings
{
  public string? FrontendUrl { get; set; } = string.Empty;
}