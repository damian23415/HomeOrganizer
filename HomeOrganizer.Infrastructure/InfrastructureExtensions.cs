using System.Data;
using HomeOrganizer.Application.Features.Repositories;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Infrastructure.Persistence;
using HomeOrganizer.Infrastructure.Persistence.Migrations;
using HomeOrganizer.Infrastructure.Security;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeOrganizer.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddMigrations(connectionString!);
        
        AddJwtSettings(services, configuration);

        return services;
    }

    private static void AddJwtSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection("Jwt").Bind(jwtSettings);
        services.AddSingleton(jwtSettings);
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
    }
}