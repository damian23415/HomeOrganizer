using HomeOrganizer.Application.Features.Users.Handlers;
using HomeOrganizer.Application.Features.Users.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeOrganizer.Application;

public static  class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Mapping.UserProfile));

        services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        
        return services;
    }
}