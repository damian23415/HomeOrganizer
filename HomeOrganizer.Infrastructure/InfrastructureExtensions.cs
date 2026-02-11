using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Infrastructure.Data;
using HomeOrganizer.Infrastructure.Repositories.Users;
using HomeOrganizer.Infrastructure.Repositories.WorkTracking;
using HomeOrganizer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeOrganizer.Infrastructure;

public static class InfrastructureExtensions
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    services.AddDbContext<HomeOrganizerDbContext>(options =>
        options.UseNpgsql(connectionString));
    
    // Repositories
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IHourlyRateRepository, HourlyRateRepository>();
    services.AddScoped<IWorkDayRepository, WorkDayRepository>();
    
    // Services
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
    services.AddScoped<IEmailService, EmailService>();
    services.AddScoped<IEmailConfirmationTokenGenerator, EmailConfirmationTokenGenerator>();
    
    
    return services;
  }
}