using FluentValidation;
using HomeOrganizer.Application.Common.Exceptions;
using HomeOrganizer.Application.Common.Models;

namespace HomeOrganizer.Api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await next(context);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred while processing the request.");
      await HandleExceptionAsync(context, ex);
    }
  }

  private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    var errorResponse = new ErrorResponse
    {
      Message = "An error occurred while processing the request.",
      Errors = [exception.Message]
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = exception switch
    {
      UserAlreadyExistsException => StatusCodes.Status409Conflict,
      ValidationException => StatusCodes.Status400BadRequest,
      _ => StatusCodes.Status500InternalServerError
    };

    await context.Response.WriteAsJsonAsync(errorResponse);
  }
}