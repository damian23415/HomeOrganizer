using System.Security.Authentication;
using HomeOrganizer.Api.Extensions;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.Users.Dtos;
using HomeOrganizer.Application.Features.Users.Interfaces;

namespace HomeOrganizer.Api.Users;

public static class UserEndpoints
{
  public static void MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/users").WithTags("Users");

    group.MapPost("register", async (
        RegisterUserRequest request,
        IUserRegistrationService userRegistrationService) =>
      {
        var result = await userRegistrationService.RegisterAsync(request);
        return Results.Ok(result);
      })
      .WithValidation<RegisterUserRequest>()
      .WithName("RegisterUser");

    group.MapPost("login", async (
        LoginRequest request,
        IUserAuthenticationService userAuthenticationService) =>
      {
        try
        {
          var result = await userAuthenticationService.LoginAsync(request);
          return Results.Ok(result);
        }
        catch (InvalidCredentialException)
        {
          var error = new ErrorResponse
          {
            Message = "Nieprawidłowy email lub hasło."
          };
          
          return Results.Json(error, statusCode: 401);
        }
      })
      .WithValidation<LoginRequest>()
      .WithName("LoginUser");
  }
}