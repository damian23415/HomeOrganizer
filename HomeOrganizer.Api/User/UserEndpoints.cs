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
                {
                    return Results.BadRequest(new ErrorResponse
                    {
                        Message = "Błąd walidacji danych",
                        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }

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
    }
}