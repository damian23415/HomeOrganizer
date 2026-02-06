using System.Data;
using HomeOrganizer.Application.Features.EmailInterfaces;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Infrastructure.Persistence.Migrations;
using HomeOrganizer.Infrastructure.Persistence.Repositories;
using HomeOrganizer.Infrastructure.Persistence.Repositories.WorkTracking;
using HomeOrganizer.Infrastructure.Security;
using HomeOrganizer.Infrastructure.Services;
using HomeOrganizer.Infrastructure.Settings;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resend;

namespace HomeOrganizer.Infrastructure;

public static class InfrastructureExtensions
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IHourlyRateRepository, HourlyRateRepository>();
    services.AddScoped<IWorkDayRepository, WorkDayRepository>();
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    
    services.AddScoped<IEmailService, EmailService>();
    services.Configure<EmailSettings>(cfg =>
        cfg.FrontendUrl = configuration["FrontendUrl"] ?? throw new NullReferenceException("FrontendUrl"));
    services.AddSingleton<IEmailSettings>(sp =>
        sp.GetRequiredService<IOptions<EmailSettings>>().Value);
    
    services.AddMigrations(connectionString!);
    AddJwtServices(services, configuration);

    var apiKey = configuration["Resend:ApiKey"]
                 ?? throw new InvalidOperationException("Resend API key not configured");

    var options = new ResendClientOptions()
    {
        ApiToken = apiKey
    };
    
    services.AddSingleton(ResendClient.Create(options));
    
    
    return services;
  }

  private static void AddJwtServices(this IServiceCollection services, IConfiguration configuration)
  {
    var jwtSettings = new JwtSettings();
    configuration.GetSection("Jwt").Bind(jwtSettings);
    
    services.AddSingleton(jwtSettings);
    services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
  }
}