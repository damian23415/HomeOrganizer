using FluentValidation;
using HomeOrganizer.Application.Common.Models;

namespace HomeOrganizer.Api.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
  private readonly IValidator<T> _validator;

  public ValidationFilter(IValidator<T> validator)
  {
    _validator = validator;
  }
  
  public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
  {
    var request = context.Arguments.OfType<T>().FirstOrDefault();
    
    if (request == null)
    {
      return Results.BadRequest(new ErrorResponse
      {
        Message = "Invalid request format",
        Errors = new List<string> { $"Expected request of type {typeof(T).Name}" }
      });
    }
    
    var validationResult = await _validator.ValidateAsync(request);
    
    if (!validationResult.IsValid)
    {
      return Results.BadRequest(new ErrorResponse
      {
        Message = "Error validating request data",
        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
      });
    }
    
    return await next(context);
  }
}