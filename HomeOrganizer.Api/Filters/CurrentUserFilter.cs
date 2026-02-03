using System.Security.Claims;
using HomeOrganizer.Application.Common.Models;

namespace HomeOrganizer.Api.Filters;

public class CurrentUserFilter : IEndpointFilter
{
  public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
  {
    var httpContext = context.HttpContext;
    
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
    {
      return ValueTask.FromResult<object?>(Results.Json(new ErrorResponse
      {
        Message = "User authentication failed"
      }, statusCode: 401));
    }
    
    httpContext.Items["UserId"] = userId;

    return ValueTask.FromResult<object?>(next(context));
  }
}