public static class InfrastructureExtensions
{
    public static IServiceCollection AddApiConfigurations(this IServiceCollection services)
    {
        services.AddSwaggerWithJwt();
        services.AuthEndpoints();

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