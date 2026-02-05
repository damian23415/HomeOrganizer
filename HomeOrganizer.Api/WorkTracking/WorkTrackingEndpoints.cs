using HomeOrganizer.Api.Extensions;
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
        IHourlyRateService hourlyRateService,
        HttpContext httpContext) =>
    {
      try
      {
        var userId = httpContext.GetCurrentUserId();
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
      catch (InvalidOperationException ex)
      {
        var error = new ErrorResponse
        {
            Message = ex.Message
        };
        
        return Results.Json(error, statusCode: 400);
      }
    })
      .WithValidation<CreateHourlyRateRequest>()
      .WithCurrentUser()
      .WithName("CreateHourlyRate");
    
    group.MapGet("hourlyRate/{date:datetime}", async (
            IHourlyRateService hourlyRateService, 
            HttpContext httpContext, 
            DateTime date) =>
        {
          try
          {
            var userId = httpContext.GetCurrentUserId();
            var response = await hourlyRateService.GetCurrentRateAsync(userId, date);
        
            return Results.Ok(response);
          }
          catch (Exception ex)
          {
            var error = new ErrorResponse()
            {
                Message = ex.Message,
            };

            return Results.Json(error, statusCode: 401);
          }
        })
        .WithCurrentUser()
        .WithName("GetCurrentHourlyRate");
    
    group.MapGet("hourlyRates", async (
        IHourlyRateService hourlyRateService,
        HttpContext context) =>
        {
          try
          {
            var userId = context.GetCurrentUserId();
            var result = await hourlyRateService.GetHourlyRates(userId);
          
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
        })
        .WithCurrentUser()
        .WithName("GetHourlyRates");
    
    group.MapPost("workDay", async (
        CreateWorkDayRequest request,
        IWorkDayService workDayService,
        HttpContext httpContext) =>
    {
      try
      {
        var userId = httpContext.GetCurrentUserId();
        var result = await workDayService.CreateWorkDayAsync(request, userId);
        
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
    })
    .WithValidation<CreateWorkDayRequest>()
    .WithCurrentUser()
    .WithName("CreateWorkDay");

    group.MapGet("workDays/{year:int}/{month:int}", async (
        int year,
        int month,
        IWorkDayService workDayService,
        HttpContext httpContext) =>
      {
        try
        {
          var userId = httpContext.GetCurrentUserId();
          var result = await workDayService.GetWorkDaysByMonthAsync(year, month, userId);
          
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
      })
      .WithCurrentUser()
      .WithName("GetWorkDaysByMonth");
  }
}