using FluentValidation;
using HomeOrganizer.Application.Common.Mappings;
using HomeOrganizer.Application.Features.Users.DTOs.Validators;
using HomeOrganizer.Application.Features.WorkTracking.DTOs.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace HomeOrganizer.Application;

public static class ApplicationExtensions
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddAutoMapper(cfg =>
    {
      cfg.AddMaps(typeof(UserMappingProfile).Assembly);
      cfg.AddMaps(typeof(HourlyRateMappingProfile).Assembly);
    });
    
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly));
    
    services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
    services.AddValidatorsFromAssemblyContaining<LoginUserRequestValidator>();
    services.AddValidatorsFromAssemblyContaining<CreateHourlyRateRequestValidator>();
    
    return services;
  }
}