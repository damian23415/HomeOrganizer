using System.Security.Authentication;
using FluentValidation;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.Users.Dtos;
using HomeOrganizer.Application.Features.Users.Interfaces;

namespace HomeOrganizer.Api.User;

public static class UserEndpoints
{
  public static void MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/users").WithTags("Users");

    group.MapPost("register", async (
        RegisterUserRequest request,
        IValidator<RegisterUserRequest> validator,
        IUserRegistrationService userRegistrationService) =>
      {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
          return Results.BadRequest(new ErrorResponse
          {
            Message = "Błąd walidacji danych",
            Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
          });

        try
        {
          var result = await userRegistrationService.RegisterAsync(request);
          return Results.Ok(result);
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
          throw;
        }
      })
      .WithName("RegisterUser");

    group.MapPost("login", async (
        LoginRequest request,
        IValidator<LoginRequest> validator,
        IUserAuthenticationService userAuthenticationService) =>
      {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
          return Results.BadRequest(new ErrorResponse
          {
            Message = "Błąd walidacji danych",
            Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
          });
        }

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
        catch (Exception e)
        {
          Console.WriteLine(e);
          throw;
        }
      })
      .WithName("LoginUser");
  }
}