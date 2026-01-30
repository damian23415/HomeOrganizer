using System.Security.Claims;
using FluentValidation;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Application.Features.WorkTracking.Interfaces;

namespace HomeOrganizer.Api.WorkTracking;

public static class WorkTrackingEndpoints
{
  public static void MapWorkTrackingEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/worktracking").WithTags("WorkTracking");

    group.MapPost("hourlyRate", async (
        CreateHourlyRateRequest request,
        IValidator<CreateHourlyRateRequest> validator,
        IHourlyRateService hourlyRateService,
        HttpContext httpContext) =>
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
          var userId = Guid.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? throw new UnauthorizedAccessException("Użytkownik niezalogowany"));

          var result = await hourlyRateService.CreateHourlyRateAsync(request, userId);
          return Results.Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
          var error = new ErrorResponse()
          {
            Message = ex.Message,
          };
          
          return Results.Json(error, statusCode: 401);
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
          throw;
        }
      })
      .RequireAuthorization()
      .WithName("CreateHourlyRate");
  }
}