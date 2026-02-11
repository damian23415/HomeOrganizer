using HomeOrganizer.Api.Extensions;
using HomeOrganizer.Application.DTOs.Users;
using HomeOrganizer.Application.Features.Users.Commands;
using HomeOrganizer.Application.Features.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;

namespace HomeOrganizer.Api.Users;

public static class UserEndpoints
{
  public static void MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/users").WithTags("Users");

    group.MapPost("register", async (
        RegisterUserRequest request,
        IMediator mediator) =>
      {
        var command = new RegisterUserCommand(request.Email, request.Password);
        var result = await mediator.Send(command);

        return !result.IsSuccess ? Results.BadRequest(result.Error) : Results.Ok(result);
      })
      .WithValidation<RegisterUserRequest>()
      .WithName("RegisterUser");

    group.MapPost("login", async (
        LoginUserRequest request,
        IMediator mediator) =>
      {
        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await mediator.Send(command);
        
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Error);
      })
      .WithValidation<LoginUserRequest>()
      .WithName("LoginUser");
    
    group.MapGet("confirmEmail", async (string token, DateTime expiry, IMediator mediator) =>
        {
          var command = new ConfirmEmailCommand(token);
          var result = await mediator.Send(command);
          
          return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Error);
        })
    .WithName("ConfirmEmail");
  }
}