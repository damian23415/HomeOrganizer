using AutoMapper;
using FluentValidation;
using HomeOrganizer.Application.Features.Users.Handlers;
using HomeOrganizer.Application.Features.Users.Interfaces;
using HomeOrganizer.Application.Features.Users.Validators;
using HomeOrganizer.Application.Features.WorkTracking.Handlers;
using HomeOrganizer.Application.Features.WorkTracking.Interfaces;
using HomeOrganizer.Application.Features.WorkTracking.Validators;
using HomeOrganizer.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeOrganizer.Application;

public static class ApplicationExtensions
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddSingleton<IMapper>(sp =>
    {
      var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

      var config = new MapperConfiguration(cfg =>
      {
        cfg.AddProfile<UserProfile>();
        cfg.AddProfile<HourlyRatePeriodProfile>();
        cfg.AddProfile<WorkDayProfile>();
      }, loggerFactory);

      return config.CreateMapper();
    });

    services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
    services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
    services.AddValidatorsFromAssemblyContaining<HourlyRateValidator>();
    services.AddValidatorsFromAssemblyContaining<WorkDayValidator>();

    services.AddScoped<IUserRegistrationService, UserRegistrationService>();
    services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
    services.AddScoped<IHourlyRateService, HourlyRateService>();
    services.AddScoped<IWorkDayService, WorkDayService>();

    return services;
  }
}