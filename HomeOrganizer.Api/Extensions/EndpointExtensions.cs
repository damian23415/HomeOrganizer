using HomeOrganizer.Api.Users;
using HomeOrganizer.Api.WorkTracking;
using Scalar.AspNetCore;

namespace HomeOrganizer.Api.Extensions;

public static class EndpointExtensions
{
  public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapOpenApi();
    app.MapUserEndpoints();
    app.MapWorkTrackingEndpoints();
    app.MapScalarApiReference();

    return app;
  }
}