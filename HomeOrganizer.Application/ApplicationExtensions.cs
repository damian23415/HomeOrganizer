using AutoMapper;
using FluentValidation;
using HomeOrganizer.Application.DTOs.Users;
using HomeOrganizer.Application.Features.Users.DTOs;
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

    // NEW

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly));
    
    services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
    
    // END NEW
    
   
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