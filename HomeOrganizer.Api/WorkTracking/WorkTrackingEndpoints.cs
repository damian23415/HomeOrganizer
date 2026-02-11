using HomeOrganizer.Api.Extensions;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Commands;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Application.Features.WorkTracking.Queries;
using MediatR;

namespace HomeOrganizer.Api.WorkTracking;

public static class WorkTrackingEndpoints
{
  public static void MapWorkTrackingEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/worktracking").WithTags("WorkTracking");

    group.MapPost("hourlyRate", async (
        CreateHourlyRateRequest request,
        IMediator mediator,
        HttpContext httpContext) =>
    {
      var userId = httpContext.GetCurrentUserId();
      var command = new CreateHourlyRateCommand(userId, request.Rate, request.EffectiveFrom, request.EffectiveTo);
      var result = await mediator.Send(command);
      
      return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    })
      .WithValidation<CreateHourlyRateRequest>()
      .WithCurrentUser()
      .WithName("CreateHourlyRate");
    
    group.MapGet("hourlyRate/{date:datetime}", async (
            IMediator mediator, 
            HttpContext httpContext, 
            DateTime date) =>
        {
          var userId = httpContext.GetCurrentUserId();
          var query = new GetCurrentHourlyRateQuery(userId, date);
          var result = await mediator.Send(query);
          
          return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .WithCurrentUser()
        .WithName("GetCurrentHourlyRate");
    
    group.MapGet("hourlyRates", async (
        IMediator mediator,
        HttpContext context) =>
        {
          var userId = context.GetCurrentUserId();
          var query = new GetAllHourlyRatesQuery(userId);
          var result = await mediator.Send(query);
          
          return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .WithCurrentUser()
        .WithName("GetHourlyRates");
    
    group.MapPost("workDay", async (
        CreateWorkDayRequest request,
        IMediator mediator,
        HttpContext httpContext) =>
        {
          var userId = httpContext.GetCurrentUserId();
          var command = new CreateWorkDayCommand(userId, request.StartTime, request.EndTime, request.Date);
          var result = await mediator.Send(command);
          
          return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .WithValidation<CreateWorkDayRequest>()
        .WithCurrentUser()
        .WithName("CreateWorkDay");

    group.MapGet("workDays/{year:int}/{month:int}", async (
        int year,
        int month,
        IMediator mediator,
        HttpContext httpContext) =>
      {
        var userId = httpContext.GetCurrentUserId();
        var query = new GetWorkDaysByMonthQuery(userId, year, month);
        var result = await mediator.Send(query);
        
        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
      })
      .WithCurrentUser()
      .WithName("GetWorkDaysByMonth");
  }
}