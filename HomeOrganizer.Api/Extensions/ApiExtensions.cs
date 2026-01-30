namespace HomeOrganizer.Api.Extensions;

public static class ApiExtensions
{
  public static IServiceCollection AddApi(this IServiceCollection services)
  {
    services.AddOpenApi();

    return services;
  }
}