using FluentValidation;
using HomeOrganizer.Application.Common.Exceptions;
using HomeOrganizer.Application.Common.Models;

namespace HomeOrganizer.Api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
  private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;
  private readonly RequestDelegate _next = next;

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas przetwarzania żądania");
      await HandleExceptionAsync(context, ex);
    }
  }

  private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    var errorResponse = new ErrorResponse
    {
      Message = "Wystąpił błąd podczas przetwarzania żądania.",
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