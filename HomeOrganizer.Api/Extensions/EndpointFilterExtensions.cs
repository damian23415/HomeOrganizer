using HomeOrganizer.Api.Filters;

namespace HomeOrganizer.Api.Extensions;

public static class EndpointFilterExtensions
{
 public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
 {
  return builder.AddEndpointFilter<ValidationFilter<T>>();
 }
 
 public static RouteHandlerBuilder WithCurrentUser(this RouteHandlerBuilder builder)
 {
  return builder
   .RequireAuthorization()
   .AddEndpointFilter<CurrentUserFilter>();
 }
 
 public static Guid GetCurrentUserId(this HttpContext httpContext)
 {
  if (httpContext.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is Guid userId)
  {
   return userId;
  }

  throw new UnauthorizedAccessException("User ID not found in context");
 }
}